using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using File = Domain.Entities.File;

namespace Infrastructure.UnitTests.Repositories.FileStorage;

[TestFixture]
public class ModifyFileTypeTests
{
    private readonly IFileStorageRepository _repository;
    
    private readonly IQueryable<File> _data = new List<File>
    {
        new()
        {
            Id = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"), 
            Type = new FileType { Id = 1, Name = "Test type 1" },
            Metadata = new FileMetadata { Visible = true }
        },
        new()
        {
            Id = Guid.Parse("ec5a9c1e-3c0c-419d-975f-e935464f38e5"), 
            Type = new FileType { Id = 2, Name = "Test type 2" },
            Metadata = new FileMetadata { Visible = false }
        },
    }.AsQueryable();

    public ModifyFileTypeTests()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Files).Returns(mockData.Object);
        _repository = new FileStorageRepository(mockContext.Object);
    }
    
    [Test]
    [TestCase("c274f552-96a5-4b2d-94e5-9038ff09236b")]
    public async Task ModifyFileType_ShouldSucceedForVisibleFile(Guid guid)
    {
        var type = new FileType { Id = 3, Name = "Test type 3" };
        var result = await _repository.ModifyFileType(guid, type).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_data.First(x => x.Id == guid).Type.Name, Is.EqualTo(type.Name));
        });
    }
    
    [Test]
    [TestCase("ec5a9c1e-3c0c-419d-975f-e935464f38e5")]
    public async Task ModifyFileType_ShouldFailForInvisibleFile(Guid guid)
    {
        var type = new FileType { Id = 4, Name = "Test type 4" };
        var result = await _repository.ModifyFileType(guid, type).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_data.First(x => x.Id == guid).Type.Name, Is.Not.EqualTo(type.Name));
        });
    }
    
    [Test]
    [TestCase("00000000-0000-0000-0000-000000000000")]
    public async Task ModifyFileType_ShouldFailForNotExistingGuid(Guid guid)
    {
        var type = new FileType { Id = 4, Name = "Test type 4" };
        var result = await _repository.ModifyFileType(guid, type).ConfigureAwait(false);

        Assert.That(result, Is.False);
    }
}