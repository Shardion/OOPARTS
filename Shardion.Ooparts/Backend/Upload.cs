namespace Shardion.Ooparts.Backend
{
    public class Upload
    {
        public string Name { get; }
        public Stream Data { get; }

        public Upload(string name, Stream data)
        {
            Name = name;
            Data = data;
        }
    }
}