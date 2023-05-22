using Amazon.S3;
using Amazon.S3.Model;

namespace DIMultiImplementationResolver
{
    public interface IAwsS3Client
    {
        /// <summary>
        /// AmazonS3Client AwsS3ClientObject { get; }
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="s3IdentifierNameWithFileName"></param>
        /// <returns></returns>

        IAmazonS3 AwsS3ClientObject { get; }

        Task<S3Object> GetFileInfo(string bucketName, string s3IdentifierNameWithFileName);

        Task<List<S3Object>> GetDirectoryInfo(string bucketName, string s3IdentifierName);

        Task<string[]> GetFilesNameInDirectory(string bucketName, string s3IdentifierName);

        Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request);

        string GetPreSignedURL(GetPreSignedUrlRequest request);

        Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request);

        Task<bool> FileExists(string bucket, string key);
    }
}
