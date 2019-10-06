using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using S3DemoApi.Models;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace S3DemoApi.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<S3Response> CreateBucketAsync(string bucketName)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName))
                    return new S3Response()
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = "Something went wrong!"
                    };

                var putBucketRequest = new PutBucketRequest()
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                var response = await _client.PutBucketAsync(putBucketRequest);

                return new S3Response()
                {
                    Status = response.HttpStatusCode,
                    Message = response.ResponseMetadata.RequestId
                };

            }
            catch (AmazonS3Exception e)
            {
                return new S3Response()
                {
                    Status = e.StatusCode,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                return new S3Response()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = e.Message
                };
            }
            
        }



        private const string FilePath = "C:\\s3TestFile.txt";
        private const string UploadWithKeyName = "UploadWithKeyName";
        private const string FileStreamUpload = "FileStreamUpload";
        private const string AdvancedUpload = "AdvancedUpload";

        public async Task<S3Response> UploadFileAsync(string bucketName)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_client);

                //Option1
                await fileTransferUtility.UploadAsync(FilePath, bucketName);

                //Option2
                await fileTransferUtility.UploadAsync(FilePath, bucketName, UploadWithKeyName);

                //Option3
                using (var fileToUpload = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    await fileTransferUtility.UploadAsync(fileToUpload, bucketName, FileStreamUpload);
                    
                }

                //Option4
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest()
                {
                    BucketName = bucketName,
                    FilePath = FilePath,
                    StorageClass = S3StorageClass.Standard,
                    PartSize = 6291456, //6mb
                    Key = AdvancedUpload,
                    CannedACL = S3CannedACL.NoACL
                };

                fileTransferUtilityRequest.Metadata.Add("param1", "value1");
                fileTransferUtilityRequest.Metadata.Add("param2", "value2");

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                return new S3Response()
                {
                    Status = HttpStatusCode.OK,
                    Message = "File(s) uploaded successfully"
                };
            }
            catch (AmazonS3Exception e)
            {
                return new S3Response()
                {
                    Status = e.StatusCode,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                return new S3Response()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = e.Message
                };
            }
        }

        public async Task<S3Response> GetObjectFromS3Async(string bucketName)
        {
            const string keyName = "s3TestFile.txt";

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                string responseBody;

                using (var response = await _client.GetObjectAsync(request))
                using(var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    var title = response.Metadata["x-amz-meta-title"];
                    var contentType = response.Headers["content-type"];

                    responseBody = reader.ReadToEnd();
                }

                var pathAndFileName = $"C:\\{keyName}";
                var createText = responseBody;
                File.WriteAllText(pathAndFileName,createText);

                return new S3Response()
                {
                    Status = HttpStatusCode.OK,
                    Message = "File(s) downloaded successfully"
                };
            }
            catch (AmazonS3Exception e)
            {
                return new S3Response()
                {
                    Status = e.StatusCode,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                return new S3Response()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = e.Message
                };
            }
        }
    }
}
