using System;

namespace Shardion.Ooparts
{
    public class UploadBatch
    {
        public IReadOnlyCollection<Upload> Uploads { get; set; }
        public DateTime CreationTimestamp { get; }

        public UploadBatch(IReadOnlyCollection<Upload> uploads)
        {
            Uploads = uploads;
            CreationTimestamp = DateTime.UtcNow;
        }
    }
}