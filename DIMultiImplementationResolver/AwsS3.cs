using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;

namespace DIMultiImplementationResolver
{
    internal class AwsS3
    {
        private const string Slash = "/";
        private static readonly string AwsAccessKeyId = "";//CommonMethod.GetAppSettingKeyValue("AwsAccessKey");
        private static readonly string AwsSecretAccessKey =""// CommonMethod.GetAppSettingKeyValue("AwsSecretAccessKey");
        private readonly AmazonS3Client awsS3Client;

        public AwsS3()
        {
            this.awsS3Client = this.Connect();
        }

        public AmazonS3Client AwsS3ClientObject
        {
            get
            {
                return this.awsS3Client;
            }
        }

        public async Task<GetObjectResponse> GetObjectFromS3Async(string bucketName, string objectKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            return await awsS3Client.GetObjectAsync(request);
        }

        public async Task<PutObjectResponse> PutObjectToS3Async(string bucketName, string objectKey, Stream fileStream)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                InputStream = fileStream
            };

            return await awsS3Client.PutObjectAsync(request);
        }

        public async Task<DeleteObjectResponse> DeleteObjectFromS3Async(string bucketName, string objectKey)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            return await awsS3Client.DeleteObjectAsync(request);
        }

        public async Task<S3ObjectInfo> GetDirectoryInfoAsync(string bucketName, string directoryPath, string delimiter = null)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = directoryPath + Slash,
                Delimiter = delimiter
            };

            var response = await awsS3Client.ListObjectsV2Async(request);

            // Filter the results to only include the specified directory
            var directory = response.CommonPrefixes.FirstOrDefault(x => x == directoryPath + Slash);

            if (directory != null)
            {
                // The specified directory was found
                var directoryInfo = new S3ObjectInfo
                {
                    Name = directory.Replace(directoryPath + Slash, string.Empty),
                    IsDirectory = true
                };

                // Retrieve the details of each object in the directory and add them to the directoryInfo object
                foreach (var obj in response.S3Objects)
                {
                    if (obj.Key.EndsWith(Slash))
                    {
                        // This is a subdirectory - skip it
                        continue;
                    }

                    // Retrieve the object metadata to get the size and last modified date
                    var metadata = await awsS3Client.GetObjectMetadataAsync(bucketName, obj.Key);
                    var objectInfo = new S3ObjectInfo
                    {
                        Name = obj.Key.Replace(directoryPath + Slash, string.Empty),
                        Size = metadata.ContentLength,
                        LastModified = metadata.LastModified,
                        IsDirectory = false
                    };

                    directoryInfo.Objects.Add(objectInfo);
                }

                return directoryInfo;
            }
            else
            {
                // The specified directory was not found
                return null;
            }
        }

        public async Task<S3ObjectInfo> GetFileInfoAsync(string bucketName, string objectKey)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            try
            {
                var response = await awsS3Client.GetObjectMetadataAsync(request);

                return new S3ObjectInfo
                {
                    BucketName = bucketName,
                    Name = objectKey,
                    Expiration = response.Expiration,
                    Expires = response.Expires,
                    IsDirectory = false,
                    Size = response.ContentLength,
                    LastModified = response.LastModified,
                };
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // The specified object was not found
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> FileExistsAsync(string bucket, string key)
        {
            try
            {
                var response = await awsS3Client.GetObjectMetadataAsync(bucket, key);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw; // re-throw any other exceptions
            }
        }

        public async Task<string[]> GetFilesNameInDirectoryAsync(string bucketName, string s3IdentifierName, string delimiter = null)
        {
            var filesName = new List<string>();
            string marker = null;

            do
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = s3IdentifierName,
                    Delimiter = delimiter,
                    ContinuationToken = marker
                };

                var response = await awsS3Client.ListObjectsV2Async(request);

                filesName.AddRange(response.S3Objects
                                    .Where(x => !x.Key.EndsWith(Slash)) // filtering only file name not folder name.
                                    .Select(x => x.Key.Split(Slash).Last()));

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

        public string GetPreSignedURL(GetPreSignedUrlRequest request)
        {
            return awsS3Client.GetPreSignedURL(request);
        }

        public string GetPreSignedURL(string bucketName, string objectKey, int expirationTimeInSeconds)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = DateTime.Now.AddSeconds(expirationTimeInSeconds)
            };
            return awsS3Client.GetPreSignedURL(request);
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
            this.awsS3Client.Dispose();
        }
    }
}
