using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;
using Shardion.Ooparts;

namespace Shardion.Ooparts.Validation
{
    public class BasicValidationLayer : IValidationLayer
    {
        public async Task<Upload?> ValidateUpload(Upload? upload)
        {
            if (upload == null)
            {
                return null;
            }
            if (upload.Data.Length > 100 * 1024 * 1024)
            {
                return null;
            }
            return upload;
        }

        public async Task<UploadBatch?> ValidateUploadBatch(UploadBatch? batch)
        {
            if (batch == null)
            {
                return null;
            }
            if ((DateTime.UtcNow - batch.CreationTimestamp).TotalMinutes > 30)
            {
                return null;
            }
            ConcurrentBag<Upload> validUploads = new();
            await Parallel.ForEachAsync<Upload>(validUploads, async (Upload upload, CancellationToken ct) => {
                Upload? validatedUpload = await ValidateUpload(upload);
                if (validatedUpload != null)
                {
                    validUploads.Add(upload);
                }
            });
            batch.Uploads = validUploads.ToArray();
            return batch;
        }
    }
}