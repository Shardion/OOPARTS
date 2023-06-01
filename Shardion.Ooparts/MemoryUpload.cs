using System.Threading.Tasks;
using System.IO;
using System;

namespace Shardion.Ooparts
{
    public class MemoryUpload : IUpload
    {
        public string Name { get; }
        public Guid Id { get; }

        public MemoryUpload(string name, Stream data)
        {
            Name = name;
            Data = data;
            Id = Guid.NewGuid();
        }

        public virtual Stream? GetData()
        {
            
        }

        public void Dispose()
        {
            (Data as IDisposable)?.Dispose();
            Data = null;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (Data is not null)
            {
                await Data.DisposeAsync();
            }
            Data = null;
            GC.SuppressFinalize(this);
        }
    }
}