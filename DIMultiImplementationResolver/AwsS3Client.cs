using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace DIMultiImplementationResolver
{
    public class AwsS3Client : IAwsS3Client, IDisposable
    {
        private static readonly string AwsAccessKeyId = GetAppSettingKeyValue("AwsAccessKey");
        private static readonly string AwsSecretAccessKey = GetAppSettingKeyValue("AwsSecretAccessKey");
        private readonly IAmazonS3 _s3Client;

        //public AwsS3Client(string accessKey, string secretKey, string region)
        //{
        //    var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

        //    _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(region));
        //}

        public AwsS3Client()
        {
            //var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            //var region = Amazon.RegionEndpoint.USWest2; //modify with your desired region

            //_s3Client = new AmazonS3Client(credentials, region);

            _s3Client = this.Connect();
        }

        public IAmazonS3 AwsS3ClientObject
        {
            get
            {
                return this._s3Client;
            }
        }

        public async Task<GetObjectResponse> GetAsync(string bucketName, string objectKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            return await _s3Client.GetObjectAsync(request);
        }

        public async Task<GetObjectResponse> GetAsync(GetObjectRequest request)
        {
            return await _s3Client.GetObjectAsync(request);
        }

        public Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request)
        {
            return this.GetAsync(request);
        }

        public async Task<PutObjectResponse> PutAsync(string bucketName, string objectKey, Stream stream)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                InputStream = stream
            };

            return await _s3Client.PutObjectAsync(request);
        }

        public async Task<PutObjectResponse> PutAsync(PutObjectRequest request)
        {
            return await _s3Client.PutObjectAsync(request);
        }

        public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request)
        {
            return this.PutAsync(request);
        }

        public async Task<bool> FileExists(string bucket, string key)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucket,
                Key = key
            };
            try
            {
                await _s3Client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw e;
            }
        }

        public string GetPreSignedURL(GetPreSignedUrlRequest request)
        {
            return _s3Client.GetPreSignedURL(request);
        }

        public string GetPreSignedURL(string bucketName, string objectKey, int expirationTimeInSeconds)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = System.DateTime.UtcNow.AddSeconds(expirationTimeInSeconds)
            };
            return _s3Client.GetPreSignedURL(request);
        }

        public virtual async Task<List<S3Object>> GetDirectoryInfoAsync(string bucketName, string s3IdentifierName)
        {
            var objects = new List<S3Object>();
            string marker = null;

            do
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = s3IdentifierName,
                    //Delimiter = "/",
                    ContinuationToken = marker
                };

                var response = await _s3Client.ListObjectsV2Async(request);

                objects.AddRange(response.S3Objects);

                if (response.IsTruncated)
                {
                    marker = response.NextContinuationToken;
                }
                else
                {
                    marker = null;
                }
            } while (marker != null);

            return objects;
        }

        //public virtual S3DirectoryInfo GetDirectoryInfo(string bucketName, string s3IdentifierName)
        //{
        //    return new S3DirectoryInfo(_s3Client, bucketName, s3IdentifierName);
        //}

        public virtual Task<List<S3Object>> GetDirectoryInfo(string bucketName, string s3IdentifierName)
        {
            return this.GetDirectoryInfoAsync(bucketName, s3IdentifierName);
        }

        public virtual async Task<S3Object> GetFileInfoAsync(string bucketName, string s3IdentifierNameWithFileName)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = s3IdentifierNameWithFileName
            };

            try
            {
                var response = await _s3Client.GetObjectMetadataAsync(request);

                return new S3Object
                {
                    BucketName = bucketName,
                    Key = s3IdentifierNameWithFileName,
                    Size = response.ContentLength
                };
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw e;
            }
        }

        //public virtual S3FileInfo GetFileInfo(string bucketName, string s3IdentifierNameWithFileName)
        //{
        //    return new S3FileInfo(_s3Client, bucketName, s3IdentifierNameWithFileName);
        //}

        public virtual Task<S3Object> GetFileInfo(string bucketName, string s3IdentifierNameWithFileName)
        {
            return this.GetFileInfoAsync(bucketName, s3IdentifierNameWithFileName);
        }

        public virtual async Task<string[]> GetFilesNameInDirectoryAsync(string bucketName, string s3IdentifierName)
        {
            var filesName = new List<string>();
            string marker = null;

            do
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = s3IdentifierName,
                    //Delimiter = "/",
                    ContinuationToken = marker
                };

                var response = await _s3Client.ListObjectsV2Async(request);

                filesName.AddRange(response.S3Objects
                                    .Where(x => !x.Key.EndsWith("/")) // filtering only file name not folder name.
                                    .Select(x => x.Key.Split('/').Last()));

                if (response.IsTruncated)
                {
                    marker = response.NextContinuationToken;
                }
                else
                {
                    marker = null;
                }
            } while (marker != null);

            return filesName.ToArray();
        }

        public virtual Task<string[]> GetFilesNameInDirectory(string bucketName, string s3IdentifierName)
        {
            return this.GetFilesNameInDirectoryAsync(bucketName, s3IdentifierName);
        }

        //public virtual string[] GetFilesNameInDirectory(string bucketName, string s3IdentifierName)
        //{
        //    var request = new ListObjectsRequest
        //    {
        //        BucketName = bucketName,
        //        Prefix = s3IdentifierName,
        //        Delimiter = "/"
        //    };

        //    var response = _s3Client.ListObjects(request);

        //    return response.S3Objects.Select(x => x.Key.Split('/').Last()).ToArray();
        //}

        public async Task<GetObjectResponse> GetObjectFromS3Async(string bucketName, string objectKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            return await _s3Client.GetObjectAsync(request);
        }

        public async Task<PutObjectResponse> PutObjectToS3Async(string bucketName, string objectKey, Stream fileStream)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                InputStream = fileStream
            };

            return await _s3Client.PutObjectAsync(request);
        }

        public async Task<DeleteObjectResponse> DeleteObjectFromS3Async(string bucketName, string objectKey)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            return await _s3Client.DeleteObjectAsync(request);
        }

        public static string GetAppSettingKeyValue(string key)
        {
            return new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build()[key];
        }

        /// <summary>
        /// Implemented Dispose method to disconnect S3 connection and to dispose the object.
        /// </summary>
        public void Dispose()
        {
            this.Disconnect();
        }

        private AmazonS3Client Connect()
        {
            return new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, RegionEndpoint.USEast2);
        }

        private void Disconnect()
        {
            this._s3Client.Dispose();
        }
    }
}
