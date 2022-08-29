using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text;
using Application.Dto;
using Application.Helpers;
using Microsoft.AspNetCore.Http;
using File = Domain.Entities.File;

namespace Application.UnitTests.Helpers;

public class FileHelperTests
{
    private IConfiguration _configuration;
    private IFileTransferService _service;
    private Guid _testGuid;

    private const string _fileContent = "some text";
    private readonly File _file = new()
    {
        Metadata = new FileMetadata
        {
            Visible = true,
            Extension = ".txt",
            Size = 1024UL,
            Name = "testfile",
            Mimetype = "application/pdf"
        },
        Type = new FileType { Name = "test1" }
    };

    public FileHelperTests()
    {
       UpdateConfig();
    }

    [SetUp]
    public async Task Init()
    {
        var content = _fileContent;
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
            ContentType = "plain/text"
        };

        var mapper = AutoMapperConfig.Initialize();
        var dto = new AddFileDto
        {
            Content = file,
            Type = "test"
        };
        var mockRepository = new Mock<IFileTransferRepository>();
        mockRepository.Setup(x => x.AddFile(It.IsAny<File>())).ReturnsAsync(Guid.NewGuid);
        mockRepository.Setup(x => x.GetFile(It.IsAny<Guid>())).ReturnsAsync(_file);
        
        _service = new FileTransferService(mockRepository.Object, mapper, _configuration);
        _file.Id = (await _service.HandleFileUpload(dto).ConfigureAwait(false)).Id;
    }

    [TearDown]
    public void UpdateConfig()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();
    }

    [Test]
    public async Task GetFileContent_ShouldReturnContent()
    {
        var content = await _file.GetFileContent(_configuration);
        Assert.That(Encoding.UTF8.GetString(content), Is.EqualTo(_fileContent));
    }
    
    [Test]
    public async Task GetFileContent_ShouldThrowIOExceptionIfDirectoryDoesNotExist()
    {
        _configuration["StoragePath"] = "/someshit/";
        Assert.ThrowsAsync<IOException>(() => _file.GetFileContent(_configuration));
    }
    
    [Test]
    public async Task GetFileContent_ShouldThrowNullRefExceptionIfConfigIsNull()
    {
        Assert.ThrowsAsync<NullReferenceException>(() => _file.GetFileContent(null));
    }

    [Test]
    public async Task MergeFiles_ShouldCreateValidZipFile()
    {
        var elementAmount = 5;
        var list = Enumerable.Range(0, elementAmount).Select(_ => _file.Id);
        var fileContents = new List<DownloadFileResultDto>();

        foreach (var item in list)
        {
            var result = await _service.HandleFileDownload(item).ConfigureAwait(false);
            fileContents.Add(result);
        }
        
        var bytes = await fileContents.MergeFiles().ConfigureAwait(false);
        using var stream = new MemoryStream(bytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        Assert.That(archive.Entries, Has.Count.EqualTo(elementAmount));
    } 
}