using System.Threading.Tasks;
using System.IO;
using System;

namespace Shardion.Ooparts
{
    public interface IUpload : IDisposable, IAsyncDisposable
    {
        string FileName { get; }
        Guid Id { get; }
        int DataLength { get; }

        Stream? OpenDataStream();
    }
}