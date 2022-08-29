namespace Application.Dto;

public class DownloadFileInputsDto
{
    public ICollection<Guid> Ids { get; set; }
    public string ArchiveName { get; set; }
}