using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interfaces;
using Moq;

namespace Application.UnitTests.Services;

[TestFixture]
public class MessageServiceTests
{
    private readonly IMessageService _service;

    private readonly IEnumerable<Message> _data = new List<Message>
    {
        new() { Id = 1, Content = "Test message 1" },
        new() { Id = 2, Content = "Test message 2" },
        new() { Id = 3, Content = "Test message 3" },
        new() { Id = 4, Content = "Test message 4" }
    };

    public MessageServiceTests()
    {
        var mockRepository = new Mock<IMessageRepository>();
        var mapper = AutoMapperConfig.Initialize();

        mockRepository.Setup(x => x.GetMessageById(It.IsAny<ulong>()))
            .ReturnsAsync((ulong id) => _data.FirstOrDefault(x => x.Id == id));
        mockRepository.Setup(x => x.GetAllMessages()).ReturnsAsync(_data.ToList());

        _service = new MessageService(mockRepository.Object, mapper);
    }

    [Test]
    [TestCase(1UL, "Test message 1")]
    [TestCase(3UL, "Test message 3")]
    public async Task ShouldCallForMessageByIdReturnMessage(ulong id, string message)
    {
        var result = await _service.GetMessageById(id).ConfigureAwait(false);
        Assert.That(result?.Content, Is.EqualTo(message));
    }

    [Test]
    public async Task ShouldReturnAllMessageElements()
    {
        var result = await _service.GetAllMessages().ConfigureAwait(false);
        Assert.That(result, Has.Count.EqualTo(_data.Count()));
    }
}