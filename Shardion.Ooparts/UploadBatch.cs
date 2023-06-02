using System.Collections.Generic;
using System;

namespace Shardion.Ooparts
{
    public class UploadBatch
    {
        public IReadOnlyCollection<IUpload> Uploads { get; set; }
        public DateTime CreationTimestamp { get; }
        public Guid Id { get; }

        public UploadBatch(IReadOnlyCollection<IUpload> uploads)
        {
            Id = Guid.NewGuid();
            Uploads = uploads;
            CreationTimestamp = DateTime.UtcNow;
        }
        public UploadBatch(IReadOnlyCollection<IUpload> uploads, Guid id)
        {
            Id = id;
            Uploads = uploads;
            CreationTimestamp = DateTime.UtcNow;
        }
    }
}