using Domain.Entities;
using File = Domain.Entities.File;

namespace Infrastructure.Interfaces;

public interface IFileTransferRepository
{
    public Task<ICollection<FileDownload>> GetFileDownloads(Guid guid);
    public Task<File> GetFile(Guid guid);
    public Task<ICollection<File>> GetFiles(ICollection<Guid> guids);
    public Task<Guid> AddFile(File file);
    public Task RemoveFile(Guid guid);
}