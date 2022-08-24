using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MessageDbContext _context;

    public MessageRepository(MessageDbContext context)
    {
        _context = context;
    }

    public async Task<Message> GetMessageById(ulong id)
    {
        return await _context.Messages.FindAsync(id).ConfigureAwait(false);
    }

    public async Task<ICollection<Message>> GetAllMessages()
    {
        return await _context.Messages.ToListAsync().ConfigureAwait(false);
    }
}