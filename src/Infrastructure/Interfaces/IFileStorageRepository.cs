using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IFileStorageRepository : IRepository
{
    public Task<bool> ModifyFileType(Guid guid, FileType fileType);
    public Task<bool> ModifyFileName(Guid guid, string name);
    public Task<bool> MarkFileAsDeleted(Guid guid);
    public Task<FileMetadata> GetFileMetadata(Guid id);
}