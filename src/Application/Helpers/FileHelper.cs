using Application.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
}