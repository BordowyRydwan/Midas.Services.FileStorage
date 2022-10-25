using Application.Dto;
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
    
    [SwaggerOperation(Summary = "Modify type of file")]
    [HttpPatch("Modify/Type", Name = nameof(ModifyFileType))]
    public async Task<IActionResult> ModifyFileType(Guid id, string type)
    {
        var result = await _fileStorageService.ModifyFileType(id, type).ConfigureAwait(false);

        if (result)
        {
            return Ok();
        }
        
        _logger.LogError("Could not found a visible file of {Id}", id);
        return NotFound();
    }
    
    [SwaggerOperation(Summary = "Modify a name of file")]
    [HttpPatch("Modify/Name", Name = nameof(ModifyFileName))]
    public async Task<IActionResult> ModifyFileName(Guid id, string name)
    {
        var result = await _fileStorageService.ModifyFileName(id, name).ConfigureAwait(false);

        if (result)
        {
            return Ok();
        }
        
        _logger.LogError("Could not found a visible file of {Id}", id);
        return NotFound();
    }
    
    [SwaggerOperation(Summary = "Get metadata of a file")]
    [HttpGet("Metadata/{id}", Name = nameof(GetFileMetadata))]
    [ProducesResponseType(typeof(FileMetadataDto), 200)]
    public async Task<IActionResult> GetFileMetadata(Guid id)
    {
        try
        {
            var result = await _fileStorageService.GetFileMetadata(id).ConfigureAwait(false);
            return Ok(result);
        }
        catch
        {
            _logger.LogError("Could not found a visible file of {Id}", id);
            return NotFound();
        }
    }
}