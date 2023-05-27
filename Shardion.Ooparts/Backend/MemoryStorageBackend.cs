using System.Collections.Generic;
using System;

namespace Shardion.Ooparts.Backend
{
    public class MemoryStorageBackend : IStorageBackend
    {
        private Dictionary<Guid, UploadBatch> _batches;

        public Guid StoreUploadBatch(UploadBatch batch)
        {
            Guid batchId = Guid.NewGuid();
            _batches[batchId] = batch;
            return batchId;
        }

        public UploadBatch? RetrieveUploadBatch(Guid batchId)
        {
            return _batches.TryGetValue(batchId, out UploadBatch batch) ? batch : null;
        }

        public void DestroyUploadBatch(Guid batchId)
        {
            _batches.Remove(batchId);
        }

        public MemoryStorageBackend()
        {
            _batches = new();
        }
    }
}