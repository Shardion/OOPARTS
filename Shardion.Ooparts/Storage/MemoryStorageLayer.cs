using System.Threading.Tasks;
using System.Collections.Concurrent;
using System;

namespace Shardion.Ooparts.Storage
{
    /// <summary>
    /// A non-asynchronous storage backend that keeps all upload batches in memory.
    /// Useful for development purposes.
    /// </summary>
    public class MemoryStorageLayer : IStorageLayer
    {
        private ConcurrentDictionary<Guid, UploadBatch> _batches;

        public Task<Guid?> StoreUploadBatch(UploadBatch batch)
        {
            Guid batchId = Guid.NewGuid();
            if (_batches.TryAdd(batchId, batch))
            {
                return Task.FromResult<Guid?>(batchId);
            }
            return Task.FromResult<Guid?>(null);
        }

        public Task<UploadBatch?> RetrieveUploadBatch(Guid batchId)
        {
            return Task.FromResult<UploadBatch?>(_batches.TryGetValue(batchId, out UploadBatch batch) ? batch : null);
        }

        public Task DestroyUploadBatch(Guid batchId)
        {
            _batches.TryRemove(batchId, out UploadBatch _);
            return Task.CompletedTask;
        }

        public MemoryStorageLayer()
        {
            _batches = new();
        }
    }
}