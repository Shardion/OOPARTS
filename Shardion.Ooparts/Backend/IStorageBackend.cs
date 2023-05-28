using System;

namespace Shardion.Ooparts.Backend
{
    public interface IStorageBackend
    {
        Task<Guid?> StoreUploadBatch(UploadBatch batch);
        Task<UploadBatch?> RetrieveUploadBatch(Guid batchId);
        Task DestroyUploadBatch(Guid batchId);
    }
}