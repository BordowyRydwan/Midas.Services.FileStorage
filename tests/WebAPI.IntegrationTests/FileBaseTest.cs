using System.Text;
using Application.Mappings;
using Application.Services;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Controllers;

namespace WebAPI.IntegrationTests;

public abstract class FileBaseTest
{
    public IFormFile GetMockFile(string contentType, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var file = new FormFile(
            baseStream: new MemoryStream(bytes),
            baseStreamOffset: 0,
            length: bytes.Length,
            name: "Data",
            fileName: "dummy.txt"
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return file;
    }
    
    public FileTransferController SetTransferController(FileDbContext dbContext, IConfiguration configuration)
    {
        var repository = new FileTransferRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var service = new FileTransferService(repository, mapper, configuration);
        var logger = Mock.Of<ILogger<FileTransferController>>();

        return new FileTransferController(logger, service);
    }

    public FileStorageController SetStorageController(FileDbContext dbContext)
    {
        var mapper = AutoMapperConfig.Initialize();
        var repository = new FileStorageRepository(dbContext);
        var service = new FileStorageService(repository, mapper);
        var logger = Mock.Of<ILogger<FileStorageController>>();

        return new FileStorageController(logger, service);
    }
}