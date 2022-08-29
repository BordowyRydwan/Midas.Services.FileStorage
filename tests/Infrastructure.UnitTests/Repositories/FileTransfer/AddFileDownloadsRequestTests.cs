using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using File = System.IO.File;

namespace Infrastructure.UnitTests.Repositories.FileTransfer;

[TestFixture]
public class AddFileDownloadsRequestTests
{
    private IFileTransferRepository _repository;
    
    private readonly IList<FileDownload> _data = new List<FileDownload>
    {
        new()
        {
            FileId = Guid.Parse("c274f552-96a5-4b2d-94e5-9038ff09236b"),
            IsSuccessful = true,
            Timestamp = DateTime.Now,
            Id = 1,
        }
    };
    
    [SetUp]
    public void Init()
    {
        var mockContext = new Mock<FileDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.FileDownloads).Returns(mockData.Object);
        mockContext.Setup(m => m.FileDownloads.AddAsync(It.IsAny<FileDownload>(), default))
            .Callback<FileDownload, CancellationToken>((file, _) => _data.Add(file));

        _repository = new FileTransferRepository(mockContext.Object);
    }
    
    [Test]
    public async Task AddDownload_ShouldAppendEntryToContext()
    {
        await _repository
            .AddFileDownloadRequest(Guid.Parse("78e616bd-fa12-47d9-909d-4f8bc00cfa6f"), true)
            .ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(_data, Has.Count.EqualTo(2));
        });
    }
}