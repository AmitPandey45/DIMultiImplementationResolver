using Amazon.S3.Model;

namespace DIMultiImplementationResolver
{
    public class S3ObjectInfo
    {
        public string BucketName { get; internal set; }
        public string Name { get; internal set; }
        public Expiration Expiration { get; internal set; }
        public DateTime Expires { get; internal set; }
        public bool IsDirectory { get; internal set; }
        public long Size { get; internal set; }
        public DateTime LastModified { get; internal set; }

        public List<S3ObjectInfo> Objects { get; set; }

        public S3ObjectInfo()
        {
            Objects = new List<S3ObjectInfo>();
        }
    }
}