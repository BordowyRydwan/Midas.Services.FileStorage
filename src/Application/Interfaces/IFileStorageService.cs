namespace Application.Interfaces;

public interface IFileStorageService : IInternalService
{
    public Task<bool> MarkFileAsDeleted(Guid guid);
}