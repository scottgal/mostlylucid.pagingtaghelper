namespace mostlylucig.pagingtaghelper.sample.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;

[Route("FilePreview")]
public class FilePreviewController : Controller
{
    private readonly IWebHostEnvironment _env;

    public FilePreviewController(IWebHostEnvironment env)
    {
        _env = env;
    }
    
    [HttpGet("Clear")]
    public IActionResult Clear()
    {
        return Content("", "text/html");
    }

    [HttpGet("GetFileContents")]
    public async Task<IActionResult> GetFileContents(string file)
    {
        if (string.IsNullOrEmpty(file))
        {
            return BadRequest("Invalid file path.");
        }

      file=  file.Replace("/", "\\");
        var fullPath = Path.Combine(_env.ContentRootPath, file);

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound("File not found.");
        }

        var fileContents =await System.IO.File.ReadAllTextAsync(fullPath);

        return Content(fileContents, "text/plain");
    }
}