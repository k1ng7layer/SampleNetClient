namespace SampleNetClient
{
    public struct OutcomePendingMessage
    {
        public readonly byte[] Payload;
        public readonly ESendMode SendMode;

        public OutcomePendingMessage(byte[] payload, ESendMode sendMode)
        {
            Payload = payload;
            SendMode = sendMode;
        }
    }
}