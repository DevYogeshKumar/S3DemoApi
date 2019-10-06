using Microsoft.AspNetCore.Mvc;
using S3DemoApi.Services;
using System.Threading.Tasks;

namespace S3DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3BucketController : ControllerBase
    {
        private readonly IS3Service _service;

        public S3BucketController(IS3Service service)
        {
            _service = service;
        }

        [HttpPost("CreateBucket/{bucketName}")]
        public async Task<IActionResult> CreateBucket([FromRoute] string bucketName)
        {
            var response = await _service.CreateBucketAsync(bucketName);

            return Ok(response);
        }

        [HttpPost]
        [Route("AddFile/{bucketName}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName)
        {
            var response = await _service.UploadFileAsync(bucketName);

            return Ok(response);
        }

        [HttpGet]
        [Route("GetFile/{bucketName}")]
        public async Task<IActionResult> GetObjectFromS3Async([FromRoute] string bucketName)
        {
            var response = await _service.GetObjectFromS3Async(bucketName);

            return Ok(response);
        }
    }
}