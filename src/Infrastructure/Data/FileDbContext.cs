using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using File = Domain.Entities.File;

namespace Infrastructure.Data;

public class FileDbContext : DbContext
{
    public virtual DbSet<File> Files { get; set; }
    public virtual DbSet<FileDownload> FileDownloads { get; set; }
    public virtual DbSet<FileType> FileTypes { get; set; }
    public virtual DbSet<FileMetadata> FileMetadatas { get; set; }

    public FileDbContext() { }
    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }
}