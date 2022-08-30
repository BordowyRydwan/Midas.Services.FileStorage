namespace Application.Dto;

public class FileDownloadInfoDto
{
    public DateTime Timestamp { get; set; }
    public bool IsSuccessful { get; set; }
    public Guid FileId { get; set; }
}