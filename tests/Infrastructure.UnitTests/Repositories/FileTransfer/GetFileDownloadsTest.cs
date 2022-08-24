using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using File = Domain.Entities.File;

namespace Infrastructure.UnitTests.Repositories.FileTransfer;

[TestFixture]
public class GetFileDownloadsTest
{
    private readonly IFileTransferRepository _repository;
    
    private readonly IQueryable<File> _data = new List<File>
    {
        new()
        {
            Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"), 
            Downloads = new List<FileDownload>
            {
                new() { IsSuccessful = true, Timestamp = DateTime.UtcNow },
                new() { IsSuccessful = true, Timestamp = DateTime.UtcNow },
                new() { IsSuccessful = false, Timestamp = DateTime.UtcNow },
                new() { IsSuccessful = true, Timestamp = DateTime.UtcNow }
            }
        },
        new()
        {
            Id = Guid.Parse("ec5a9c1e-3c0c-419d-975f-e935464f38e5"),
        },
    }.AsQueryable();

    public GetFileDownloadsTest()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Files).Returns(mockData.Object);
        _repository = new FileTransferRepository(mockContext.Object);
    }

    [Test]
    [TestCase("c274f552-96a5-4b2d-94e5-9038ff09236b")]
    
    public async Task GetFileDownloads_ShouldGetDownloadList(Guid guid)
    {
        var fileDownloads = await _repository.GetFileDownloads(guid).ConfigureAwait(false);
        var testedInstance = _data.FirstOrDefault(x => x.Id == guid);
        
        Assert.That(fileDownloads, Is.EquivalentTo(testedInstance.Downloads));
    }
    
    [Test]
    [TestCase("ec5a9c1e-3c0c-419d-975f-e935464f38e5")]
    public async Task GetFileDownloads_ShouldReturnEmptyListIfNoEntries(Guid guid)
    {
        var fileDownloads = await _repository.GetFileDownloads(guid).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fileDownloads, Is.Not.Null);
            Assert.That(fileDownloads, Has.Count.Zero);
        });
    }
    
    [Test]
    [TestCase("00000000-0000-0000-0000-000000000000")]
    public async Task GetFileDownloads_ShouldReturnEmptyListIfFileDoesNotExist(Guid guid)
    {
        var fileDownloads = await _repository.GetFileDownloads(guid).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fileDownloads, Is.Not.Null);
            Assert.That(fileDownloads, Has.Count.Zero);
        });
    }
}