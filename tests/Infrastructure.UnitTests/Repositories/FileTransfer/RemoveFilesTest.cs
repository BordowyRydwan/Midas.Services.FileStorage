using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using File = Domain.Entities.File;

namespace Infrastructure.UnitTests.Repositories.FileTransfer;

[TestFixture]
public class RemoveFilesTest
{
    private readonly IFileTransferRepository _repository;

    private readonly List<File> _data = new()
    {
        new File
        {
            Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"),
            Metadata = new FileMetadata { Visible = true },
            Type = new FileType { Name = "test1" },
        }
    };

    public RemoveFilesTest()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockData.Setup(x => x.FindAsync(It.IsAny<Guid>())).ReturnsAsync((object[] ids) =>
        {
            var id = (Guid)ids[0];
            return _data.FirstOrDefault(x => x.Id == id);
        });
        
        mockData.Setup(m => m.Remove(It.IsAny<File>()))
            .Callback<File>(file => _data.Remove(file));
        
        mockContext.Setup(x => x.Files).Returns(mockData.Object);
        _repository = new FileTransferRepository(mockContext.Object);
    }

    [Test]
    public async Task RemoveFile_ShouldRemoveFileFromContext()
    {
        var guid = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b");

        await _repository.RemoveFile(guid).ConfigureAwait(false);
        Assert.That(_data, Has.Count.Zero);
    }
}