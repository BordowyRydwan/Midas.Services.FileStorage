using System.Text;
using Application.Dto;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using File = Domain.Entities.File;

namespace Application.UnitTests.Services;

[TestFixture]
public class FileTransferServiceTests
{
    private readonly IFileTransferService _service;
    
    public FileTransferServiceTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        configuration.GetSection("StoragePath").Value = null;
        var mockRepository = new Mock<IFileTransferRepository>();
        var mapper = AutoMapperConfig.Initialize();

        mockRepository.Setup(x => x.AddFile(It.IsAny<File>())).ReturnsAsync(Guid.NewGuid);

        _service = new FileTransferService(mockRepository.Object, mapper, configuration);
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
    public async Task AddFile_ShouldReturnUnsuccessfulResultDtoModel_WhenFileSaveErrorOccurs()
    {
        var faultyFile = GetMockFile("text/plain", "test");
        var dto = new AddFileDto
        {
            Content = faultyFile,
            Type = "test"
        };
        var result = await _service.HandleFileUpload(dto).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Id, Is.EqualTo(Guid.Empty));
        });
    }
    
    [Test]
    public async Task AddFile_ShouldReturnUnsuccessfulResultDtoModel_WhenContentOrTypeAreNull()
    {
        var dto = new AddFileDto
        {
            Content = null,
            Type = null
        };
        var result = await _service.HandleFileUpload(dto).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Id, Is.EqualTo(Guid.Empty));
        });
    }
}