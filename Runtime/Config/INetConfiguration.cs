namespace SampleNetClient.Runtime.Config
{
    public interface INetConfiguration
    {
        string IpAddress { get; }
        int Port { get; }
    }
}