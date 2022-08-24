namespace Domain.Entities;

public class File
{
    public Guid Id { get; set; }
    
    public FileMetadata Metadata { get; set; }
    public ulong MetadataId { get; set; }
    public FileType Type { get; set; }
    public ulong TypeId { get; set; }
    public ICollection<FileDownload> Downloads { get; set; }
}