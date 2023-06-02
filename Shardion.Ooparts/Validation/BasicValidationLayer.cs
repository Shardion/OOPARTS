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
            if (upload is null)
            {
                return Task.FromResult<IUpload?>(null);
            }
            else if (upload.DataLength > 100 * 1024 * 1024 || upload.DataLength < 4)
            {
                return Task.FromResult<IUpload?>(null);
            }
            else
            {
                return Task.FromResult<IUpload?>(upload);
            }
        }

        public async Task<UploadBatch?> ValidateUploadBatch(UploadBatch? batch)
        {
            if (batch is null)
            {
                return null;
            }
            if ((DateTime.UtcNow - batch.CreationTimestamp).TotalMinutes > 30)
            {
                return null;
            }
            List<IUpload> validUploads = new();
            foreach (IUpload upload in batch.Uploads)
            {
                IUpload? validatedUpload = await ValidateUpload(upload);
                if (validatedUpload is not null)
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