using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileStorageController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<FileStorageController> _logger;

    public FileStorageController(ILogger<FileStorageController> logger, IFileStorageService fileStorageService)
    {
        _logger = logger;
        _fileStorageService = fileStorageService;
    }
    
    [SwaggerOperation(Summary = "Mark file as deleted")]
    [HttpPatch("MarkAsDeleted", Name = nameof(MarkFileAsDeleted))]
    public async Task<IActionResult> MarkFileAsDeleted(Guid id)
    {
        var result = await _fileStorageService.MarkFileAsDeleted(id).ConfigureAwait(false);

        if (result)
        {
            return Ok();
        }
        
        _logger.LogError("Could not found a visible file of {Id}", id);
        return NotFound();
    }
}