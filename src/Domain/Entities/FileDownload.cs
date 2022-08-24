namespace Domain.Entities;

public class FileDownload
{
    public ulong Id { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsSuccessful { get; set; }

    public File File { get; set; }
    public Guid FileId { get; set; }
}