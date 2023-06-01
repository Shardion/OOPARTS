using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.AspNetCore.Http;
using Shardion.Ooparts;

namespace Shardion.Ooparts.Validation
{
    public class BasicValidationLayer : IValidationLayer
    {
        public Task<IUpload?> ValidateUpload(IUpload? upload)
        {
            if (upload == null)
            {
                return Task.FromResult<IUpload?>(null);
                Console.WriteLine("scrubbed upload: null");
            }
            else if (upload.Length > 100 * 1024 * 1024 || upload.Length < 4)
            {
                return Task.FromResult<IUpload?>(null);
                Console.WriteLine($"scrubbed upload for size: {upload}");
            }
            else
            {
                return Task.FromResult<Upload?>(upload);
                Console.WriteLine($"validated upload: {upload}");
            }
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
            List<Upload> validUploads = new();
            foreach (Upload upload in batch.Uploads)
            {
                Upload? validatedUpload = await ValidateUpload(upload);
                if (validatedUpload != null)
                {
                    validUploads.Add(validatedUpload);
                }
                else
                {
                    Console.WriteLine($"invalidated upload: {upload}");
                }
            }
            batch.Uploads = validUploads.ToArray();
            return batch;
        }
    }
}