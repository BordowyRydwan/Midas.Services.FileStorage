using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using File = Domain.Entities.File;

namespace Infrastructure.UnitTests.Repositories.FileStorage;

[TestFixture]
public class MarkFileAsDeletedTests
{
    private readonly IFileStorageRepository _repository;
    
    private readonly IQueryable<File> _data = new List<File>
    {
        new()
        {
            Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"),
            Metadata = new FileMetadata()
            {
                Name = "test1",
                Extension = "pdf",
                Mimetype = "application/pdf",
                Size = 2137UL,
                UploadDate = DateTime.UtcNow,
                Visible = true
            }
        },
        new()
        {
            Id = Guid.Parse("ec5a9c1e-3c0c-419d-975f-e935464f38e5"),
            Metadata = new FileMetadata()
            {
                Name = "test2",
                Extension = "png",
                Mimetype = "image/png",
                Size = 1488UL,
                UploadDate = DateTime.UtcNow,
                Visible = false
            }
        }
    }.AsQueryable();
    
    public MarkFileAsDeletedTests()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Files).Returns(mockData.Object);
        _repository = new FileStorageRepository(mockContext.Object);
    }
    
    [Test]
    [TestCase("c274f552-96a5-4b2d-94e5-9038ff09236b")]
    public async Task MarkFileAsDeleted_ShouldSucceedForVisibleFile(Guid guid)
    {
        var result = await _repository.MarkFileAsDeleted(guid).ConfigureAwait(false);
        var testedInstance = _data.First(x => x.Id == guid);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(testedInstance.Metadata.Visible, Is.False);
        });
    }
    
    [Test]
    [TestCase("ec5a9c1e-3c0c-419d-975f-e935464f38e5")]
    public async Task MarkFileAsDeleted_ShouldDoNothingForInvisibleFile(Guid guid)
    {
        var result = await _repository.MarkFileAsDeleted(guid).ConfigureAwait(false);
        var testedInstance = _data.First(x => x.Id == guid);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(testedInstance.Metadata.Visible, Is.False);
        });
    }
    
    [Test]
    [TestCase("00000000-0000-0000-0000-000000000000")]
    public async Task MarkFileAsDeleted_ShouldFailForNotExistingGuid(Guid guid)
    {
        var result = await _repository.MarkFileAsDeleted(guid).ConfigureAwait(false);

        Assert.That(result, Is.False);
    }
}