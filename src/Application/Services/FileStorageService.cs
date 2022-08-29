using Application.Interfaces;
using AutoMapper;
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
}