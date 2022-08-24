using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using File = Domain.Entities.File;

namespace Infrastructure.UnitTests.Repositories.FileTransfer;

[TestFixture]
public class GetFilesTest
{
    private readonly IFileTransferRepository _repository;
    
    private readonly IQueryable<File> _data = new List<File>
    {
        new()
        {
            Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"), 
            Metadata = new FileMetadata
            {
                Visible = true
            },
            Type = new FileType { Name = "test1" },
            Downloads = new List<FileDownload>
            {
                new ()
            }
        },
        new()
        {
            Id = Guid.Parse("ec5a9c1e-3c0c-419d-975f-e935464f38e5"),
            Metadata = new FileMetadata
            {
                Visible = false
            },
            Type = new FileType { Name = "test1" },
            Downloads = new List<FileDownload>
            {
                new ()
            }
        },
        new()
        {
            Id = Guid.Parse("280fbb1c-9812-4be1-89c8-4e58605c2abf"),
            Metadata = new FileMetadata
            {
                Visible = true
            },
            Type = new FileType { Name = "test1" },
            Downloads = new List<FileDownload>
            {
                new ()
            }
        },
    }.AsQueryable();

    public GetFilesTest()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Files).Returns(mockData.Object);
        _repository = new FileTransferRepository(mockContext.Object);
    }

    [Test]
    [TestCase("c274f552-96a5-4b2d-94e5-9038ff09236b")]
    public async Task GetFile_AreAllPropertiesIncluded(Guid guid)
    {
        var file = await _repository.GetFile(guid).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(file.Downloads, Is.Not.Null);
            Assert.That(file.Type, Is.Not.Null);
            Assert.That(file.Metadata, Is.Not.Null);
        });
    }
    
    [Test]
    [TestCase("ec5a9c1e-3c0c-419d-975f-e935464f38e5")]
    public async Task GetFile_IsResultNullWhenFileIsNotVisible(Guid guid)
    {
        var file = await _repository.GetFile(guid).ConfigureAwait(false);
        
        Assert.That(file, Is.Null);
    }
    
    [Test]
    [TestCase("00000000-0000-0000-0000-000000000000")]
    public async Task GetFile_IsResultNullWhenFileDoesNotExist(Guid guid)
    {
        var file = await _repository.GetFile(guid).ConfigureAwait(false);
        
        Assert.That(file, Is.Null);
    }

    [Test]
    public async Task GetFiles_DoesResultContainOnlyVisibleFiles()
    {
        var testCases = _data.Select(x => x.Id).ToList();
        var file = await _repository.GetFiles(testCases).ConfigureAwait(false);
        var result = _data.Count(x => x.Metadata.Visible);
        
        Assert.That(result, Is.EqualTo(file.Count));
    }
}