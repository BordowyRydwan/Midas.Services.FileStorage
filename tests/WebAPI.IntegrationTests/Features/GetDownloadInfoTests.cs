using Application.Dto;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebAPI.Controllers;

namespace WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class GetDownloadInfoTests : FileBaseTest
{
    private readonly FileTransferController _controller;
    private Guid _downloadId;
    
    public GetDownloadInfoTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("FileStorageConnection");

        var dbOptions = new DbContextOptionsBuilder<FileDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new FileDbContext(dbOptions);
        
        _controller = SetTransferController(dbContext, configuration);
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

        var result = await _controller.AddFile(dto).ConfigureAwait(false) as OkObjectResult;
        _downloadId = (result.Value as AddFileResultDto).Id;

        await _controller.DownloadFile(_downloadId).ConfigureAwait(false);
    }
    
    [Test]
    public async Task GetDownloads_ShouldReturnHTTP200()
    {
        var result = await _controller.GetFileDownloads(_downloadId).ConfigureAwait(false);
        
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetDownloads_ShouldReturnAListWithOneDownloadForTestFile()
    {
        var result = await _controller.GetFileDownloads(_downloadId).ConfigureAwait(false) as OkObjectResult;
        var value = result.Value as FileDownloadInfoListDto;
        
        Assert.Multiple(() =>
        {
            Assert.That(value, Has.Count.EqualTo(1));
            Assert.That(value.Items.Select(x => x.IsSuccessful), Is.All.True);
        });
    }
    [Test]
    public async Task GetDownloads_ShouldReturnHTTP400ForWrongId()
    {
        var result = await _controller.GetFileDownloads(Guid.Empty).ConfigureAwait(false);
        
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
    
}