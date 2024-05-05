using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace SampleNetClient.Runtime
{
    public class LiteNetRUDPTransport : Transport, INetEventListener
    {
        private NetManager _clientManager;
        private NetDataWriter _writer;
        private NetPeer _server;
        private bool _running;

        public override event Action PeerConnected;
        public override event Action<byte[]> DataReceived;

        public override void Start(IPEndPoint remoteEndpoint)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            _writer = new NetDataWriter();
            _clientManager = new NetManager(this);
            _clientManager.Start();
            _clientManager.Connect(remoteEndpoint, _writer);
            _running = true;
        }

        public override void Tick()
        {
            if (!_running)
                return;
            
            _clientManager.PollEvents();
        }

        public override void Stop()
        {
            _running = false;
            _clientManager.Stop();
        }

        public override void Send(byte[] data, ESendMode sendMode)
        {
            var msgLength = BitConverter.GetBytes(data.Length);
            var buffer = new byte[msgLength.Length + data.Length];
            
            Buffer.BlockCopy(msgLength, 0, buffer, 0, msgLength.Length);
            Buffer.BlockCopy(data, 0, buffer, 4, data.Length);
            
            _server.Send(buffer, DeliveryMethod.ReliableOrdered);
            
            Debug.Log($"Transport send data: {data.Length}");
        }

        public override void Dispose()
        {
            Stop();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("Connected to server: " + peer.Address);
            _server = peer;
            
            PeerConnected?.Invoke();
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("Disconnected from server: " + disconnectInfo.Reason);
            _server = null;
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log("[C] NetworkError: " + socketError);
        }

        public void OnNetworkReceive(
            NetPeer peer, 
            NetPacketReader reader, 
            byte channelNumber, 
            DeliveryMethod deliveryMethod
        )
        {
            if (peer.RemoteId != _server.RemoteId)
                return;
            
            var mgLengthBytes = new Span<byte>(reader.RawData, 4, 4);
            var msgLength = BitConverter.ToUInt16(mgLengthBytes);
            var msg = new Span<byte>(reader.RawData, 8, msgLength);
            
            DataReceived?.Invoke(msg.ToArray());
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }
    }
}