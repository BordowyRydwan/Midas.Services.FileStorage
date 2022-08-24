using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessageController> _logger;

    public MessageController(ILogger<MessageController> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    [SwaggerOperation(Summary = "Get message by ID")]
    [ProducesResponseType(typeof(MessageDto), 200)]
    [HttpGet("{id}", Name = nameof(GetMessageById))]
    public async Task<IActionResult> GetMessageById(ulong id)
    {
        var result = await _messageService.GetMessageById(id).ConfigureAwait(false);

        if (result is not null)
        {
            return Ok(result);
        }

        _logger.LogError("User tried to get a message with id: {Id}, and was unable to find it", id);
        return NotFound();
    }

    [SwaggerOperation(Summary = "Get all messages")]
    [ProducesResponseType(typeof(MessageListDto), 200)]
    [HttpGet("All", Name = nameof(GetAllMessages))]
    public async Task<IActionResult> GetAllMessages()
    {
        var result = await _messageService.GetAllMessages().ConfigureAwait(false);

        if (result is not null)
        {
            return Ok(result);
        }

        _logger.LogError("User tried to get a message list and was unable to get it");
        return NotFound();
    }
}