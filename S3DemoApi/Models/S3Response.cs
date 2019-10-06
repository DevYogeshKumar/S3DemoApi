using System.Net;

namespace S3DemoApi.Models
{
    public class S3Response
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
