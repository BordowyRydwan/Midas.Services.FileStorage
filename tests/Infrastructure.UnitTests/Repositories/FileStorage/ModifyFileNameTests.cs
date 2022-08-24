using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories.FileStorage.FileStorage;

[TestFixture]
public class ModifyFileNameTests
{
    private readonly IFileStorageRepository _repository;
    
    private readonly IQueryable<FileMetadata> _data = new List<FileMetadata>
    {
        new()
        {
            Name = "test1",
            Extension = "pdf",
            FileId = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"),
            Mimetype = "application/pdf",
            Size = 2137UL,
            UploadDate = DateTime.UtcNow,
            Visible = true,
        },
        new()
        {
            Name = "test2",
            Extension = "png",
            FileId = Guid.Parse("ec5a9c1e-3c0c-419d-975f-e935464f38e5"),
            Mimetype = "image/png",
            Size = 1488UL,
            UploadDate = DateTime.UtcNow,
            Visible = false,
        }
    }.AsQueryable();
    
    public ModifyFileNameTests()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.FileMetadatas).Returns(mockData.Object);
        _repository = new FileStorageRepository(mockContext.Object);
    }
    
    [Test]
    [TestCase("c274f552-96a5-4b2d-94e5-9038ff09236b")]
    public async Task ModifyFileName_ShouldSucceedForVisibleFile(Guid guid)
    {
        var newName = "test325";
        var result = await _repository.ModifyFileName(guid, newName).ConfigureAwait(false);
        var testedInstance = _data.First(x => x.FileId == guid);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(testedInstance.Name, Is.EqualTo(newName));
        });
    }
    
    [Test]
    [TestCase("ec5a9c1e-3c0c-419d-975f-e935464f38e5")]
    public async Task ModifyFileName_ShouldFailForInvisibleFile(Guid guid)
    {
        var newName = "test325";
        var result = await _repository.ModifyFileName(guid, newName).ConfigureAwait(false);
        var testedInstance = _data.First(x => x.FileId == guid);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(testedInstance.Name, Is.Not.EqualTo(newName));
        });
    }
    
    [Test]
    [TestCase("00000000-0000-0000-0000-000000000000")]
    public async Task ModifyFileName_ShouldFailForNotExistingGuid(Guid guid)
    {
        var result = await _repository.ModifyFileName(guid, string.Empty).ConfigureAwait(false);

        Assert.That(result, Is.False);
    }
}