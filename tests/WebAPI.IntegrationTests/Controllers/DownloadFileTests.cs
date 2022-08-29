using System.Text;
using Application.Dto;
using Application.Mappings;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Controllers;

namespace WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class DownloadFileTests
{
    private readonly FileTransferController _controller;
    private Guid _downloadId = Guid.Empty;
    
    public DownloadFileTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("FileStorageConnection");

        var dbOptions = new DbContextOptionsBuilder<FileDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new FileDbContext(dbOptions);
        var repository = new FileTransferRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var service = new FileTransferService(repository, mapper, configuration);
        var logger = Mock.Of<ILogger<FileTransferController>>();

        _controller = new FileTransferController(logger, service);
    }

    [OneTimeSetUp]
    public async Task Init()
    {
        var file = GetMockFile("text/plain", "some dummy text");
        var dto = new AddFileDto
        {
            Content = file,
            Type = "document"
        };

        var result = await _controller.AddFile(dto).ConfigureAwait(false) as OkObjectResult;
        _downloadId = (result.Value as AddFileResultDto).Id;
    }

    private IFormFile GetMockFile(string contentType, string content)
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

    [Test]
    public async Task DownloadFile_ShouldReturnHTTP200IfDownloadIsSuccessful()
    {
        var response = await _controller.DownloadFile(_downloadId).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<FileContentResult>());
    }
    
    [Test]
    public async Task DownloadFile_ShouldReturnHTTP404IfWrongGuidPasted()
    {
        var response = await _controller.DownloadFile(Guid.NewGuid()).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }
    
    [Test]
    public async Task DownloadFile_ShouldReturnHTTP400IfSomethingBadOccursDuringDownload()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("FileStorageConnection");

        var dbOptions = new DbContextOptionsBuilder<FileDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new FileDbContext(dbOptions);
        var repository = new FileTransferRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var service = new FileTransferService(repository, mapper, null);
        var logger = Mock.Of<ILogger<FileTransferController>>();

        var mockController = new FileTransferController(logger, service);
        
        var response = await mockController.DownloadFile(_downloadId).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }
}