using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using File = Domain.Entities.File;

namespace Infrastructure.Repositories;

public class FileTransferRepository : IFileTransferRepository
{
    private readonly FileDbContext _context;

    public FileTransferRepository(FileDbContext context)
    {
        _context = context;
    }
    
    public async Task<ICollection<FileDownload>> GetFileDownloads(Guid guid)
    {
        var file = await _context.Files
            .Include(x => x.Downloads)
            .SingleOrDefaultAsync(x => x.Id == guid)
            .ConfigureAwait(false);

        return file?.Downloads ?? new List<FileDownload>();
    }

    public async Task<File> GetFile(Guid guid)
    {
        var file = await _context.Files
            .Include(x => x.Metadata)
            .Include(x => x.Downloads)
            .Include(x => x.Type)
            .SingleOrDefaultAsync(x => x.Id == guid && x.Metadata.Visible)
            .ConfigureAwait(false);

        return file;
    }

    public async Task<ICollection<File>> GetFiles(ICollection<Guid> guids)
    {
        var files = await _context.Files
            .Include(x => x.Metadata)
            .Include(x => x.Downloads)
            .Include(x => x.Type)
            .Where(x => guids.Contains(x.Id) && x.Metadata.Visible)
            .ToListAsync()
            .ConfigureAwait(false);

        return files;
    }

    public async Task<Guid> AddFile(File file)
    {
        if (file.Type is null)
        {
            return Guid.Empty;
        }
        
        var type = await _context.FileTypes.SingleOrDefaultAsync(x => x.Name == file.Type.Name);

        if (type is not null)
        {
            file.Type = type;
        }

        await _context.Files.AddAsync(file).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return file.Id;
    }

    public async Task RemoveFile(Guid guid)
    {
        var entity = await _context.Files.FindAsync(guid).ConfigureAwait(false);
        
        _context.Files.Remove(entity);
    }
}