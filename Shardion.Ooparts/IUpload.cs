using System.Threading.Tasks;
using System.IO;
using System;

namespace Shardion.Ooparts
{
    public interface IUpload : IDisposable, IAsyncDisposable
    {
        string Name { public get; }
        Guid Id { public get; }

        Stream? OpenDataStream();
    }
}