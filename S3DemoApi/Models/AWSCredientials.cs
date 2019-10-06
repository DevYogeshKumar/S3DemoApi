using Amazon.Runtime;

namespace S3DemoApi.Models
{
    public class Credentials : AWSCredentials
    {
        public override ImmutableCredentials GetCredentials()
        {
            throw new System.NotImplementedException();
        }
    }
}
