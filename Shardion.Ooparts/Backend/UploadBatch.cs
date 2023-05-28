namespace Shardion.Ooparts.Backend
{
    public class UploadBatch
    {
        public IReadOnlyCollection<Upload> Uploads { get; }
        public DateTime CreationTimestamp { get; }

        public UploadBatch(IReadOnlyCollection<Upload> uploads)
        {
            CreationTimestamp = DateTime.UtcNow;
        }
    }
}