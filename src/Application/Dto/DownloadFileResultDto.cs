namespace Application.Dto;

public class DownloadFileResultDto
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public byte[] Content { get; set; }
    public string Mimetype { get; set; }
    public bool SuccessfullyDownloaded { get; set; }
    public bool Found { get; set; }
}