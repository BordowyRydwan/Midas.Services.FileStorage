using Application.Dto;

namespace Application.Interfaces;

public interface IMessageService : IInternalService
{
    public Task<MessageDto> GetMessageById(ulong id);
    public Task<MessageListDto> GetAllMessages();
}