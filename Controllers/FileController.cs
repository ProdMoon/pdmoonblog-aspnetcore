using Microsoft.AspNetCore.Mvc;
using PdmoonblogApi.Interfaces;

namespace PdmoonblogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IAwsS3Service _awsS3Service;

    public FileController(IAwsS3Service awsS3Service)
    {
        _awsS3Service = awsS3Service;
    }

    // POST: api/File/upload
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFileAsync([FromForm] IFormFile file, [FromForm] string key)
    {
        await using var stream = file.OpenReadStream();
        await _awsS3Service.UploadFileAsync(key, stream);

        return Ok();
    }

    // GET: api/File/download/{key}
    [HttpGet("download/{key}")]
    public async Task<IActionResult> DownloadFileAsync(string key)
    {
        var stream = await _awsS3Service.DownloadFileAsync(key);

        return File(stream, "application/octet-stream");
    }

    // GET: api/File/list
    [HttpGet("list")]
    public async Task<IActionResult> ListFilesAsync()
    {
        var fileKeys = await _awsS3Service.ListFilesAsync();

        return Ok(fileKeys);
    }
}