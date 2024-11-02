using Microsoft.AspNetCore.Mvc;
using PdmoonblogApi.Interfaces;

namespace PdmoonblogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;


    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    // POST: api/File/upload
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFileAsync([FromForm] IFormFile file, [FromForm] string key)
    {
        await using var stream = file.OpenReadStream();
        await _fileService.UploadFileAsync(key, stream);

        return Ok();
    }

    // GET: api/File/download/{key}
    [HttpGet("download/{key}")]
    public async Task<IActionResult> DownloadFileAsync(string key)
    {
        var stream = await _fileService.DownloadFileAsync(key);

        return File(stream, "application/octet-stream");
    }
}