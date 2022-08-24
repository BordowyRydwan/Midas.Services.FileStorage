using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class MessageServiceTests
{
    private readonly IMessageRepository _repository;

    private readonly IQueryable<Message> _data = new List<Message>
    {
        new() { Id = 1, Content = "Test message 1" },
        new() { Id = 2, Content = "Test message 2" },
        new() { Id = 3, Content = "Test message 3" },
        new() { Id = 4, Content = "Test message 4" }
    }.AsQueryable();

    public MessageServiceTests()
    {
        var mockContext = new Mock<MessageDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockData.Setup(x => x.FindAsync(It.IsAny<ulong>())).ReturnsAsync((object[] ids) =>
        {
            var id = (ulong)ids[0];
            return _data.FirstOrDefault(x => x.Id == id);
        });
        mockContext.Setup(x => x.Messages).Returns(mockData.Object);

        _repository = new MessageRepository(mockContext.Object);
    }

    [Test]
    [TestCase(1UL, "Test message 1")]
    [TestCase(3UL, "Test message 3")]
    public async Task ShouldCallForMessageByIdReturnMessage(ulong id, string message)
    {
        var result = await _repository.GetMessageById(id).ConfigureAwait(false);
        Assert.That(result?.Content, Is.EqualTo(message));
    }

    [Test]
    public async Task ShouldReturnAllMessageElements()
    {
        var result = await _repository.GetAllMessages().ConfigureAwait(false);
        Assert.That(result, Has.Count.EqualTo(_data.Count()));
    }
}