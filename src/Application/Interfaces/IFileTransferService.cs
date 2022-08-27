using Application.Dto;

namespace Application.Interfaces;

public interface IFileTransferService
{
    public Task<AddFileResultDto> HandleFileUpload(AddFileDto file);
}