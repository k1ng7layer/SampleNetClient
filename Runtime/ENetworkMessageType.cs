namespace SampleNetClient.Runtime
{
    public enum ENetworkMessageType
    {
        ConnectionRequest,
        ClientDisconnected,
        ClientConnected,
        ClientReconnected,
        ClientReady,
        AuthenticationResult,
        NetworkMessage,
        ClientAliveCheck,
        ServerAliveCheck,
        Ping,
        Sync,
        None
    }
}