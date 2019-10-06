using S3DemoApi.Models;
using System.Threading.Tasks;

namespace S3DemoApi.Services
{
    public interface IS3Service
    {
        Task<S3Response> CreateBucketAsync(string bucketName);

        Task<S3Response> UploadFileAsync(string bucketName);
        Task<S3Response> GetObjectFromS3Async(string bucketName);
    }
}