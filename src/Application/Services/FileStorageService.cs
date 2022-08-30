using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IFileStorageRepository _fileRepository;

    public FileStorageService(IFileStorageRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<bool> MarkFileAsDeleted(Guid id)
    {
        return await _fileRepository.MarkFileAsDeleted(id).ConfigureAwait(false);
    }

    public async Task<bool> ModifyFileType(Guid id, string type)
    {
        var fileType = new FileType { Name = type };

        return await _fileRepository.ModifyFileType(id, fileType).ConfigureAwait(false);
    }

    public async Task<bool> ModifyFileName(Guid id, string name)
    {
        return await _fileRepository.ModifyFileName(id, name).ConfigureAwait(false);
    }
}