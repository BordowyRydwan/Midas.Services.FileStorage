using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Infrastructure.Interfaces;

namespace Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessageService(IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<MessageDto> GetMessageById(ulong id)
    {
        var repositoryResult = await _messageRepository.GetMessageById(id).ConfigureAwait(false);
        var mappedDto = _mapper.Map<MessageDto>(repositoryResult);

        return mappedDto;
    }

    public async Task<MessageListDto> GetAllMessages()
    {
        var repositoryResult = await _messageRepository.GetAllMessages().ConfigureAwait(false);
        var mappedDto = _mapper.Map<MessageListDto>(repositoryResult);

        return mappedDto;
    }
}