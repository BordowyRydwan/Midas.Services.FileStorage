using Application.Dto;

namespace Application.Interfaces;

public interface IFileTransferService : IInternalService
{
    public Task<AddFileResultDto> HandleFileUpload(AddFileDto file);
    public Task<DownloadFileResultDto> HandleFileDownload(Guid id);
    public Task<DownloadFileResultDto> HandleFilesDownload(DownloadFileInputsDto files);
    public Task<FileDownloadInfoListDto> GetFileDownloads(Guid id);
}