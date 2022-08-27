using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using File = Domain.Entities.File;

namespace Infrastructure.UnitTests.Repositories.FileTransfer;

[TestFixture]
public class AddFilesTest
{
    private IFileTransferRepository _repository;

    private IList<File> _data = new List<File>
    {
        new()
        {
            Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"),
            Metadata = new FileMetadata { Visible = true },
            Type = new FileType { Name = "test1" },
        }
    };
    
    private readonly IList<FileType> _fileTypes = new List<FileType>
    {
        new()
        {
            Id = 1,
            Name = "test",
        },
        new()
        {
            Id = 2,
            Name = "document",
        },
    };

    [SetUp]
    public void Init()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        var mockFileTypes = _fileTypes.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Files).Returns(mockData.Object);
        mockContext.Setup(x => x.FileTypes).Returns(mockFileTypes.Object);
        mockContext.Setup(m => m.Files.AddAsync(It.IsAny<File>(), default))
            .Callback<File, CancellationToken>((file, _) =>
            {
                file.Id = Guid.NewGuid();
                _data.Add(file);
            });

        _repository = new FileTransferRepository(mockContext.Object);
    }

    [TearDown]
    public void ClearList()
    {
        _data = new List<File>
        {
            new()
            {
                Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"),
                Metadata = new FileMetadata { Visible = true },
                Type = new FileType { Name = "test1" },
            }
        };
    }

    [Test]
    public async Task AddFile_ShouldAppendFileToContext()
    {
        var fileToAdd = new File
        {
            Metadata = new FileMetadata { Visible = true },
            Type = new FileType { Name = "test" },
        };

        var addFileResult = await _repository.AddFile(fileToAdd).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(addFileResult, Is.Not.EqualTo(Guid.Empty));
            Assert.That(_data, Has.Count.EqualTo(2));
        });
    }
    
    [Test]
    public async Task AddFile_ShouldReturnEmptyGuidAndNotAddFileWhenTypeIsEmpty()
    {
        var fileToAdd = new File
        {
            Metadata = new FileMetadata { Visible = true },
        };

        var addFileResult = await _repository.AddFile(fileToAdd).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(addFileResult, Is.EqualTo(Guid.Empty));
            Assert.That(_data, Has.Count.EqualTo(1));
        });
    }
}