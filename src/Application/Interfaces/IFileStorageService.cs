namespace Application.Interfaces;

public interface IFileStorageService : IInternalService
{
    public Task<bool> MarkFileAsDeleted(Guid guid);
    public Task<bool> ModifyFileType(Guid id, string type);
    public Task<bool> ModifyFileName(Guid id, string name);
}