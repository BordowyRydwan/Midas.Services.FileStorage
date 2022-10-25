namespace Application.Dto;

public class FileMetadataDto
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public string Mimetype { get; set; }
    public ulong Size { get; set; }
    public bool Visible { get; set; }
    public DateTime UploadDate { get; set; }
}