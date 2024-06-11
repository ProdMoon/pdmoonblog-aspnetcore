namespace PdmoonblogApi.Interfaces;

public interface IAwsS3Service
{
    Task UploadFileAsync(string key, Stream inputStream);
    Task<Stream> DownloadFileAsync(string key);
    Task<List<string>> ListFilesAsync();
}
