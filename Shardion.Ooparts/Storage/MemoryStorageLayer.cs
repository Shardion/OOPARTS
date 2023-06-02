using System.Threading.Tasks;
using System.Collections.Generic;
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
            List<IUpload> uploads = new();
            foreach (IUpload upload in batch.Uploads)
            {
                Stream? uploadStream = upload.OpenDataStream();
                if (uploadStream != null)
                {
                    byte[] uploadData = new byte[upload.DataLength];
                    await uploadStream.ReadAsync(uploadData, 0, upload.DataLength);
                    uploads.Add(new MemoryUpload(upload.FileName, uploadData));
                }
            }

            UploadBatch modifiedBatch = new(uploads.ToArray(), batch.Id);

            if (_batches.TryAdd(modifiedBatch.Id, modifiedBatch))
            {
                return modifiedBatch.Id;
            }
            else
            {
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