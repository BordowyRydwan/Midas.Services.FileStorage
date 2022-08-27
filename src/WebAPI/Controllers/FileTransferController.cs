using Application.Dto;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/File")]
public class FileTransferController : ControllerBase
{
    private readonly IFileTransferService _fileTransferService;
    private readonly ILogger<FileTransferController> _logger;

    public FileTransferController(ILogger<FileTransferController> logger, IFileTransferService fileTransferService)
    {
        _logger = logger;
        _fileTransferService = fileTransferService;
    }
    
    [SwaggerOperation(Summary = "Add file to File Storage Service")]
    [ProducesResponseType(typeof(AddFileResultDto), 200)]
    [HttpPost("Add", Name = nameof(AddFile))]
    public async Task<IActionResult> AddFile([FromForm] AddFileDto fileDto)
    {
        var result = await _fileTransferService.HandleFileUpload(fileDto).ConfigureAwait(false);

        if (result.Success)
        {
            return Ok(result);
        }
        
        _logger.LogError("Could not upload a file");
        return BadRequest();
    }
}