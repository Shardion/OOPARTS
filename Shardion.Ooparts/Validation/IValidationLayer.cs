namespace Shardion.Ooparts.Validation
{
    public interface IValidationLayer
    {
        Task<Upload?> ValidateUpload(Upload? upload);
        Task<UploadBatch?> ValidateUploadBatch(UploadBatch? upload);
    }
}