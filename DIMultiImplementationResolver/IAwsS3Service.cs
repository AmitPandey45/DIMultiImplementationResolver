using Amazon.S3.Model;

namespace DIMultiImplementationResolver
{
    public interface IAwsS3Service
    {
        Stream GetFileStream(string s3BucketName, string s3IdentifierName, string fileName);

        string GetPreSignedURL(string s3BucketName, string s3IdentifierName, string fileName);

        PutObjectResponse UploadFile(string s3BucketName, string s3IdentifierName, string fileName, MemoryStream stream);

        GetObjectResponse DownloadFile(string s3BucketName, string s3IdentifierName, string fileName);
    }
}
