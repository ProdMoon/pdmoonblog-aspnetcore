namespace PdmoonblogApi.Interfaces;

public interface IFileService
{
    Task UploadFileAsync(string key, Stream inputStream);
    Task<Stream> DownloadFileAsync(string key);
}
