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
    private readonly IFileTransferRepository _repository;

    private readonly IList<File> _data = new List<File>
    {
        new()
        {
            Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"),
            Metadata = new FileMetadata { Visible = true },
            Type = new FileType { Name = "test1" },
        }
    };

    public AddFilesTest()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Files).Returns(mockData.Object);
        mockContext.Setup(m => m.Files.AddAsync(It.IsAny<File>(), default))
            .Callback<File, CancellationToken>((file, _) =>
            {
                file.Id = Guid.NewGuid();
                _data.Add(file);
            });

        _repository = new FileTransferRepository(mockContext.Object);
    }

    [Test]
    public async Task AddFile_ShouldAppendFileToContext()
    {
        var fileToAdd = new File
        {
            Metadata = new FileMetadata { Visible = true },
            Type = new FileType { Name = "test1" },
        };

        var addFileResult = await _repository.AddFile(fileToAdd).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(addFileResult, Is.Not.EqualTo(Guid.Empty));
            Assert.That(_data, Has.Count.EqualTo(2));
        });
    }
}