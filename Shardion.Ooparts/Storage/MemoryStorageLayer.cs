using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System;

namespace Shardion.Ooparts.Storage
{
    /// <summary>
    /// A storage backend that keeps all upload batches in memory.
    /// Useful for development purposes.
    /// </summary>
    public class MemoryStorageLayer : IStorageLayer
    {
        private ConcurrentDictionary<Guid, UploadBatch> _batches;

        public async Task<Guid?> StoreUploadBatch(UploadBatch batch)
        {
            foreach (Upload upload in batch.Uploads)
            {
                MemoryStream copiedUploadStream = new();
                await upload.Data.CopyToAsync(copiedUploadStream);
                upload.Data = copiedUploadStream;
            }

            if (_batches.TryAdd(batch.Id, batch))
            {
                return batch.Id;
            }
            else
            {
                foreach (Upload upload in batch.Uploads)
                {
                    upload.Data.Close();
                }
                return null;
            }
        }

        public Task<UploadBatch?> RetrieveUploadBatch(Guid batchId)
        {
            _batches.TryGetValue(batchId, out UploadBatch? batch);
            return Task.FromResult<UploadBatch?>(batch);
        }

        public Task DestroyUploadBatch(Guid batchId)
        {
            _batches.TryRemove(batchId, out UploadBatch? _);
            return Task.CompletedTask;
        }

        public MemoryStorageLayer()
        {
            _batches = new();
        }
    }
}