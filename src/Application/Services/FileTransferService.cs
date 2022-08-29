using Application.Dto;
using Application.Helpers;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
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

    public async Task<DownloadFileResultDto> HandleFileDownload(Guid id)
    {
        var content = Array.Empty<byte>();
        var isDownloadSuccessful = true;
        var fileInfo = await _fileRepository.GetFile(id).ConfigureAwait(false);

        if (fileInfo is null)
        {
            return new DownloadFileResultDto { Found = false };
        }

        try
        {
            content = await fileInfo.GetFileContent(_configuration).ConfigureAwait(false);
        }
        catch
        {
            isDownloadSuccessful = false;
        }
        
        await _fileRepository.AddFileDownloadRequest(id, isDownloadSuccessful);

        return _mapper.Map<FileMetadata, DownloadFileResultDto>(fileInfo.Metadata, opt =>
            opt.AfterMap((_, dest) =>
            {
                dest.Content = content;
                dest.SuccessfullyDownloaded = isDownloadSuccessful;
                dest.Found = true;
            }
        ));
    }

    public async Task<DownloadFileResultDto> HandleFilesDownload(DownloadFileInputsDto files)
    {
        var fileContents = new List<DownloadFileResultDto>();
        var isDownloadSuccessful = false;
        var archive = Array.Empty<byte>();

        foreach (var id in files.Ids)
        {
            var result = await HandleFileDownload(id).ConfigureAwait(false);

            if (!result.Found)
            {
                return new DownloadFileResultDto
                {
                    Found = false
                };
            }

            if (result.SuccessfullyDownloaded)
            {
                fileContents.Add(result);
            }
        }

        if (fileContents.Count == files.Ids.Count)
        {
            archive = await fileContents.MergeFiles().ConfigureAwait(false);
            isDownloadSuccessful = true;
        }

        return new DownloadFileResultDto
        {
            Content = archive,
            Extension = ".zip",
            Mimetype = "application/zip",
            Name = files.ArchiveName,
            SuccessfullyDownloaded = isDownloadSuccessful,
            Found = true
        };
    }
}