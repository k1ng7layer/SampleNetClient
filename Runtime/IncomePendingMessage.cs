using System;

namespace SampleNetClient.Runtime
{
    public struct IncomePendingMessage
    {
        public readonly byte[] Payload;

        public IncomePendingMessage(byte[] payload)
        {
            Payload = payload;
        }
    }
}