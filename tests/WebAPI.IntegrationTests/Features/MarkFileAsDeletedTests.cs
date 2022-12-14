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
using NUnit.Framework;
using WebAPI.Controllers;

namespace WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class MarkFileAsDeletedTests : FileBaseTest
{
    private readonly FileTransferController _transferController;
    private readonly FileStorageController _storageController;
    private Guid _fileId = Guid.Empty;

    public MarkFileAsDeletedTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("FileStorageConnection");

        var dbOptions = new DbContextOptionsBuilder<FileDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new FileDbContext(dbOptions);
        
        _transferController = SetTransferController(dbContext, configuration);
        _storageController = SetStorageController(dbContext);
    }
    
    [SetUp]
    public async Task Init()
    {
        var file = GetMockFile("text/plain", "some dummy text");
        var dto = new AddFileDto
        {
            Content = file,
            Type = "document"
        };

        var result = await _transferController.AddFile(dto).ConfigureAwait(false) as OkObjectResult;
        _fileId = (result.Value as AddFileResultDto).Id;
    }
    
    [Test]
    public async Task SuccessfullyMarkedFile_ShouldReturnHTTP200()
    {
        var response = await _storageController.MarkFileAsDeleted(_fileId).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<OkResult>());
    }

    [Test]
    public async Task FailDuringMarkedFile_ShouldReturnHTTP404()
    {
        var response = await _storageController.MarkFileAsDeleted(Guid.NewGuid()).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }
}