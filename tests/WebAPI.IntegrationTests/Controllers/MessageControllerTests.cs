using Application.Dto;
using Application.Mappings;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebAPI.Controllers;
using Moq;

namespace WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class MessageControllerTests
{
    private readonly MessageController _messageController;

    public MessageControllerTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var dbOptions = new DbContextOptionsBuilder<MessageDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new MessageDbContext(dbOptions);
        var repository = new MessageRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var service = new MessageService(repository, mapper);
        var logger = Mock.Of<ILogger<MessageController>>();

        _messageController = new MessageController(logger, service);
    }

    [Test]
    [TestCase(1UL)]
    [TestCase(3UL)]
    public async Task ExistingIdShouldReturnHTTP200(ulong id)
    {
        var response = await _messageController.GetMessageById(id).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    [TestCase(10UL)]
    [TestCase(30UL)]
    public async Task NotExistingIdShouldReturnHTTP404(ulong id)
    {
        var response = await _messageController.GetMessageById(id).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task ShouldReturnAllMessageElements()
    {
        var result = await _messageController.GetAllMessages().ConfigureAwait(false) as OkObjectResult;
        Assert.That(result.Value as MessageListDto, Has.Count.GreaterThan(0));
    }
}