using System;
using System.Net;

namespace SampleNetClient.Runtime
{
    public abstract class Transport : IDisposable
    {
        public abstract event Action PeerConnected;
        public abstract event Action<byte[]> DataReceived;
        public abstract void Start(IPEndPoint remoteEndpoint);
        public abstract void Tick();
        public abstract void Stop();
        public abstract void Send(byte[] data, ESendMode sendMode);
        
        public abstract void Dispose();
    }
}