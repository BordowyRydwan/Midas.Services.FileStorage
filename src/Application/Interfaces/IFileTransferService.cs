using Application.Dto;

namespace Application.Interfaces;

public interface IFileTransferService
{
    public Task<AddFileResultDto> HandleFileUpload(AddFileDto file);
    public Task<DownloadFileResultDto> HandleFileDownload(Guid id);
    public Task<DownloadFileResultDto> HandleFilesDownload(DownloadFileInputsDto files);
}