using System.Threading.Tasks;
using System.IO;
using System;

namespace Shardion.Ooparts
{
    public sealed class Upload : IDisposable, IAsyncDisposable
    {
        public string Name { get; }
        public Stream Data { get; private set; }

        public Upload(string name, Stream data)
        {
            Name = name;
            Data = data;
        }

        public void Dispose()
        {
            (Data as IDisposable)?.Dispose();
            Data = null;
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
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