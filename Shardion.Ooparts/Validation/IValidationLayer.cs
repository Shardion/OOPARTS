using System.Threading.Tasks;
using Shardion.Ooparts;

namespace Shardion.Ooparts.Validation
{
    public interface IValidationLayer
    {
        Task<IUpload?> ValidateUpload(IUpload? upload);
        Task<UploadBatch?> ValidateUploadBatch(UploadBatch? upload);
    }
}