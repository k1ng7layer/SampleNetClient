namespace SampleNetClient.Runtime
{
    public class NetClient
    {
        public int ID { get; }
        public bool IsLocal { get; set; }

        public NetClient(int id)
        {
            ID = id;
        }
    }
}