using Application.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;
using File = System.IO.File;

namespace Application.Helpers;
public static class FileHelper
{
    public static async Task SaveFile(this IFormFile file, IConfiguration configuration, string fileName)
    {
        var storagePath = configuration.GetSection("StoragePath").Value;
        
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }

        var filePath = Path.Combine(storagePath, fileName);
        
        await using var fileStream = File.Create(filePath);
        await file.CopyToAsync(fileStream);
    }

    public static Domain.Entities.File MapFormFileToEntity(this AddFileDto fileDto)
    {
        var metadata = new FileMetadata
        {
            Extension = Path.GetExtension(fileDto.Content.FileName),
            Mimetype = fileDto.Content.ContentType,
            Name = Path.GetFileNameWithoutExtension(fileDto.Content.FileName),
            Size = Convert.ToUInt64(fileDto.Content.Length),
            UploadDate = DateTime.UtcNow,
            Visible = true
        };

        return new Domain.Entities.File
        {
            Metadata = metadata,
            Type = new FileType { Name = fileDto.Type }
        };
    }

    public static async Task<byte[]> GetFileContent(this Domain.Entities.File file, IConfiguration configuration)
    {
        var storagePath = configuration.GetSection("StoragePath").Value;
        
        if (!Directory.Exists(storagePath))
        {
            throw new IOException();
        }
        
        var filePath = Path.Combine(storagePath, $"{file.Id}{file.Metadata.Extension}");

        await using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream);

        return reader.ReadBytes((int)file.Metadata.Size);
    }

    public static async Task<byte[]> MergeFiles(this List<DownloadFileResultDto> files)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
        {
            foreach (var file in files)
            {
                var fileFullName = Path.ChangeExtension(file.Name, file.Extension);
                var fileEntry = archive.CreateEntry(fileFullName, CompressionLevel.Optimal);

                await using var entryStream = fileEntry.Open();
                await entryStream.WriteAsync(file.Content, 0, file.Content.Length);
            }
        }

        return stream.ToArray();
    }
}