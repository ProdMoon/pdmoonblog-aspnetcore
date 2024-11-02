using System.IO;
using System.Threading.Tasks;
using PdmoonblogApi.Interfaces;

namespace PdmoonblogApi.Services;

public class FileService : IFileService
{
    private readonly string _baseDirectory;
    
    public FileService(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }

    public async Task UploadFileAsync(string key, Stream inputStream)
    {
        string filePath = Path.Combine(_baseDirectory, key);
        using (var fileStream = File.Create(filePath))
        {
            await inputStream.CopyToAsync(fileStream);
        }
    }

    public async Task<Stream> DownloadFileAsync(string key)
    {
        string filePath = Path.Combine(_baseDirectory, key);
        if (!File.Exists(filePath))
        {
            return null;
        }

        var outputStream = new MemoryStream();
        using (var fileStream = File.OpenRead(filePath))
        {
            await fileStream.CopyToAsync(outputStream);
        }

        outputStream.Position = 0;
        return outputStream;
    }
}
