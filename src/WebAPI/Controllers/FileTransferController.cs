using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    
    [SwaggerOperation(Summary = "Download file from File Storage Service")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [HttpPatch("Download/Single", Name = nameof(DownloadFile))]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var result = await _fileTransferService.HandleFileDownload(id).ConfigureAwait(false);

        if (!result.Found)
        {
            _logger.LogError("Could not find a file");
            return NotFound(); 
        }

        if (!result.SuccessfullyDownloaded)
        {
            _logger.LogError("An error occurred during downloading a file present in database");
            return BadRequest(); 
        }
        
        return File(
            fileContents: result.Content, 
            contentType: result.Mimetype, 
            fileDownloadName: result.Name + result.Extension
        );
    }
    
    [SwaggerOperation(Summary = "Download multiple files from File Storage Service")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [HttpPatch("Download/Multiple", Name = nameof(DownloadFiles))]
    public async Task<IActionResult> DownloadFiles(DownloadFileInputsDto fileDto)
    {
        var result = await _fileTransferService.HandleFilesDownload(fileDto).ConfigureAwait(false);
        
        if (!result.Found)
        {
            _logger.LogError("Could not find at least one of selected files");
            return NotFound(); 
        }

        if (!result.SuccessfullyDownloaded)
        {
            _logger.LogError("An error occurred during downloading a set of files present in database");
            return BadRequest();
        }
        
        return File(
            fileContents: result.Content, 
            contentType: result.Mimetype, 
            fileDownloadName: result.Name + result.Extension
        );
    }
    
    [SwaggerOperation(Summary = "Get info about file downloads from File Storage Service")]
    [ProducesResponseType(typeof(FileDownloadInfoListDto), 200)]
    [HttpGet("Download/GetEntries", Name = nameof(GetFileDownloads))]
    public async Task<IActionResult> GetFileDownloads(Guid id)
    {
        var result = await _fileTransferService.GetFileDownloads(id).ConfigureAwait(false);

        if (result.Count != 0)
        {
            return Ok(result);
        }
        
        _logger.LogError("Could not find any download request on this file");
        return NotFound();
    }
}