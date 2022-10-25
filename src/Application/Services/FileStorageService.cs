using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IFileStorageRepository _fileRepository;
    private readonly IMapper _mapper;

    public FileStorageService(IFileStorageRepository fileRepository, IMapper mapper)
    {
        _fileRepository = fileRepository;
        _mapper = mapper;
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

    public async Task<FileMetadataDto> GetFileMetadata(Guid id)
    {
        var fileMetadata = await _fileRepository.GetFileMetadata(id).ConfigureAwait(false);
        var mappedMetadata = _mapper.Map<FileMetadata, FileMetadataDto>(fileMetadata);

        return mappedMetadata;
    }
}