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

public class AddFileTests
{
    private readonly FileTransferController _controller;

    public AddFileTests()
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
    
    [Test]
    public async Task SuccessfullyAddedFile_ShouldReturnHTTP200()
    {
        var file = GetMockFile("text/plain", "some dummy text");
        var dto = new AddFileDto
        {
            Content = file,
            Type = "document"
        };
        var response = await _controller.AddFile(dto).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task FailDuringAddingFile_ShouldReturnHTTP400()
    {
        var dto = new AddFileDto
        {
            Content = null,
            Type = null,
        };
        
        var response = await _controller.AddFile(dto).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<BadRequestResult>());
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
}