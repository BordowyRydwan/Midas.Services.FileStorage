using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FileStorageRepository : IFileStorageRepository
{
    private readonly FileDbContext _context;

    public FileStorageRepository(FileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ModifyFileType(Guid guid, FileType fileType)
    {
        var file = await _context.Files
            .Include(x => x.Type)
            .Include(x => x.Metadata)
            .SingleOrDefaultAsync(x => x.Id == guid && x.Metadata.Visible)
            .ConfigureAwait(false);

        if (file is null)
        {
            return false;
        }
        
        file.Type = fileType;
        _context.Update(file);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    public async Task<bool> ModifyFileName(Guid guid, string name)
    {
        var file = await _context.Files
            .Include(x => x.Metadata)
            .SingleOrDefaultAsync(x => x.Id == guid && x.Metadata.Visible)
            .ConfigureAwait(false);

        if (file is null)
        {
            return false;
        }
        
        file.Metadata.Name = name;

        _context.Update(file);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    public async Task<bool> MarkFileAsDeleted(Guid id)
    {
        var file = await _context.Files
            .Include(x => x.Metadata)
            .SingleOrDefaultAsync(x => x.Id == id && x.Metadata.Visible)
            .ConfigureAwait(false);
        
        if (file is null)
        {
            return false;
        }
        
        file.Metadata.Visible = false;
        _context.Update(file);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    
}