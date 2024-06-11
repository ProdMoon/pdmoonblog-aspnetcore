using PdmoonblogApi.Interfaces;
using Amazon.S3;
using Amazon.S3.Model;
using PdmoonblogApi.Models;

namespace PdmoonblogApi.Services;

public class AwsS3Service : IAwsS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "pdmoonblogbucket";

    public AwsS3Service(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task UploadFileAsync(string key, Stream inputStream)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = inputStream
        };

        var response = await _s3Client.PutObjectAsync(putRequest);
    }

    public async Task<Stream> DownloadFileAsync(string key)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        var response = await _s3Client.GetObjectAsync(getRequest);
        return response.ResponseStream;
    }

    public async Task<List<string>> ListFilesAsync()
    {
        var listRequest = new ListObjectsV2Request
        {
            BucketName = _bucketName
        };

        var response = await _s3Client.ListObjectsV2Async(listRequest);
        return response.S3Objects.Select(o => o.Key).ToList();
    }
}
