using Application.Dto;
using Application.Helpers;
using Application.Interfaces;
using AutoMapper;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class FileTransferService : IFileTransferService
{
    private readonly IFileTransferRepository _fileRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public FileTransferService(IFileTransferRepository fileRepository, IMapper mapper, IConfiguration configuration)
    {
        _fileRepository = fileRepository;
        _mapper = mapper;
        _configuration = configuration;
    }
    
    public async Task<AddFileResultDto> HandleFileUpload(AddFileDto file)
    {
        var fileIdentifier = Guid.Empty;
        
        if (file.Content is null || string.IsNullOrEmpty(file.Type))
        {
            return _mapper.Map<Guid, AddFileResultDto>(fileIdentifier);
        }

        var fileEntity = file.MapFormFileToEntity();
        fileIdentifier = await _fileRepository.AddFile(fileEntity).ConfigureAwait(false);
        var fileName = Path.ChangeExtension(fileIdentifier.ToString(), fileEntity.Metadata.Extension);
        
        try
        {
            await file.Content.SaveFile(_configuration, fileName).ConfigureAwait(false);
        }
        catch
        {
            await _fileRepository.RemoveFile(fileIdentifier);
            fileIdentifier = Guid.Empty;
        }

        return _mapper.Map<Guid, AddFileResultDto>(fileIdentifier);
    }
}