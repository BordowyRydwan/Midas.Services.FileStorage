using Application.Interfaces;

namespace Application.Dto;

public class FileDownloadInfoListDto : IListDto<FileDownloadInfoDto>
{
    public int Count { get; set; }
    public ICollection<FileDownloadInfoDto> Items { get; set; }
}