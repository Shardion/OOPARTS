using System.Threading.Tasks;
using System.IO;
using System;

namespace Shardion.Ooparts
{
    public class MemoryUpload : IUpload
    {
        public string FileName { get; }
        public Guid Id { get; }
        public byte[] Data { get; }
        public int DataLength { get; }

        public MemoryUpload(string fileName, byte[] data)
        {
            FileName = Path.GetFileName(fileName);
            Data = data;
            DataLength = data.Length;
            Id = Guid.NewGuid();
        }

        public virtual Stream? OpenDataStream()
        {
            return new MemoryStream(Data, false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
        }
    }
}