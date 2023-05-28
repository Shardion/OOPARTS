using System.Threading.Tasks;
using System;
using Shardion.Ooparts;

namespace Shardion.Ooparts.Storage
{
    public interface IStorageLayer
    {
        Task<Guid?> StoreUploadBatch(UploadBatch batch);
        Task<UploadBatch?> RetrieveUploadBatch(Guid batchId);
        Task DestroyUploadBatch(Guid batchId);
    }
}