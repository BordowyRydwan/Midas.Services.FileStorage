using Application.Dto;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using WebAPI.Controllers;

namespace WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class ModifyFileNameTests : FileBaseTest
{
    private readonly FileTransferController _transferController;
    private readonly FileStorageController _storageController;
    private Guid _fileId = Guid.Empty;
    private string _name = "testname";

    public ModifyFileNameTests()
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
    public async Task SuccessfullyModifiedFilename_ShouldReturnHTTP200()
    {
        var response = await _storageController.ModifyFileName(_fileId, _name).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<OkResult>());
    }

    [Test]
    public async Task FailDuringModifyingFilename_ShouldReturnHTTP404()
    { 
        var response = await _storageController.ModifyFileName(Guid.NewGuid(), _name).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }
}