using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IMessageRepository : IRepository
{
    public Task<Message> GetMessageById(ulong id);
    public Task<ICollection<Message>> GetAllMessages();
}