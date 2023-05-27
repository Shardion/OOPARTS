using System;

namespace Shardion.Ooparts.Backend
{
    public interface IStorageBackend
    {
        Guid StoreUploadBatch(UploadBatch batch);
        UploadBatch? RetrieveUploadBatch(Guid batchId);
        void DestroyUploadBatch(Guid batchId);
    }
}