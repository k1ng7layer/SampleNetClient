using System;

namespace SampleNetClient.Runtime
{
    public static class MessageHelper
    {
        internal static ENetworkMessageType ReadMessageType(ArraySegment<byte> data)
        {
            var messageTypeSpan = data.Slice(0, 2);
            var flagsInt = BitConverter.ToUInt16(messageTypeSpan);

            var result = ENetworkMessageType.None;

            if (Enum.TryParse(flagsInt.ToString(), out ENetworkMessageType messageType))
                result = messageType;
            
            return result;
        }
    }
}