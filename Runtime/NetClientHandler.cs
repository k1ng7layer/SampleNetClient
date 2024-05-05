using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SampleNetClient.Runtime
{
    public class NetClientHandler : IDisposable
    {
        private readonly Transport _transport;
        private readonly Queue<OutcomePendingMessage> _outcomePendingMessages = new();
        private readonly Queue<IncomePendingMessage> _incomePendingMessages = new();
        private bool _running;

        public NetClientHandler(Transport transport)
        {
            _transport = transport;
        }
        
        public NetClient Client { get; private set; }
        public event Action<EConnectionResult, string> ConnResultReceived;

        public void Connect(IPEndPoint remoteEndpoint, string payload = "")
        {
            _transport.Start(remoteEndpoint);
            
            _transport.PeerConnected += OnConnectionEstablished;
            _transport.DataReceived += OnRawData;
            _running = true;
            void OnConnectionEstablished()
            {
                _transport.PeerConnected -= OnConnectionEstablished;
                
                var pwdBytes = Encoding.UTF8.GetBytes(payload);
                var writer = new ByteWriter(sizeof(ushort) + pwdBytes.Length);
            
                writer.AddUshort((ushort)ENetworkMessageType.ConnectionRequest);
                writer.AddString(payload);
            
                Send(writer.Data, ESendMode.Reliable);
            }
        }
        
        public void Dispose()
        {
            _transport.DataReceived -= OnRawData;
            _transport.Dispose();
        }

        public void Stop()
        {
            _running = false;
            _transport.Stop();
        }

        public void Tick()
        {
            if (!_running)
                return;
            
            _transport.Tick();
            
            ReceiveQueue();
            SendQueue();
        }
        
        private void SendQueue()
        {
            while (_outcomePendingMessages.Count > 0)
            {
                var msg = _outcomePendingMessages.Dequeue();
                
                _transport.Send(msg.Payload, msg.SendMode);
            }
        }
        
        private void ReceiveQueue()
        {
            while (_incomePendingMessages.Count > 0)
            {
                var msg = _incomePendingMessages.Dequeue();

                HandleIncomeMsg(msg.Payload);
            }
        }

        private void OnRawData(byte[] data)
        {
            var msg = new IncomePendingMessage(data);
            
            _incomePendingMessages.Enqueue(msg);
        }

        private void HandleIncomeMsg(byte[] data)
        {
            var msgType = MessageHelper.ReadMessageType(data);

            switch (msgType)
            {
                case ENetworkMessageType.ClientDisconnected:
                    break;
                case ENetworkMessageType.ClientConnected:
                    break;
                case ENetworkMessageType.ClientReconnected:
                    break;
                case ENetworkMessageType.AuthenticationResult:
                    OnAuthResult(data);
                    break;
                case ENetworkMessageType.NetworkMessage:
                    break;
                case ENetworkMessageType.ServerAliveCheck:
                    break;
                case ENetworkMessageType.Ping:
                    break;
                case ENetworkMessageType.Sync:
                    break;
            }
        }

        public void Send(byte[] data, ESendMode sendMode)
        {
            var message = new OutcomePendingMessage(data, sendMode);
            _outcomePendingMessages.Enqueue(message);
        }

        private void OnAuthResult(byte[] data)
        {
            var byteReader = new ByteReader(data, 2);
            var result = (EConnectionResult)byteReader.ReadUshort();
            var clientId = byteReader.ReadInt32();
            var reason = byteReader.ReadString(out _);

            if (result == EConnectionResult.Success)
            {
                Client = new NetClient(clientId);
                Client.IsLocal = true;
            }

            if (result == EConnectionResult.Reject) Stop();
            
            ConnResultReceived?.Invoke(result, reason);
        }
    }
}