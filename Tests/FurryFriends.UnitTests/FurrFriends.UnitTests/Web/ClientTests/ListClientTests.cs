using Ardalis.Result;
using FastEndpoints;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Domain.Clients.Query.ListClients;
using FurryFriends.Web.Endpoints.ClientEndpoints.List;
using Moq;

namespace FurryFriends.UnitTests.Web.ClientTests;

public class ListClientTests
{
  private readonly Mock<IMediator> _mediator;
  private readonly Mock<ILogger<ListClient>> _logger;

  public ListClientTests()
  {
    _mediator = new Mock<IMediator>();
    _logger = new Mock<ILogger<ListClient>>();
  }

  [Fact]
  public async Task HandleAsync_ReturnsClientList_WhenSuccessful()
  {
    // Arrange
    var request = new ListClientRequest { Page = 1, PageSize = 10, SearchTerm = "test" };
    var clients = GetFakeClients();
    var result = Result<List<ClientDTO>>.Success(clients);

    _mediator.Setup(m => m.Send(It.IsAny<ListClientQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);
    var endpoint = Factory.Create<ListClient>(_mediator.Object, _logger.Object);

    // Act
    await endpoint.HandleAsync(request, CancellationToken.None);

    // Assert
    endpoint.Response.Should().NotBeNull();
    endpoint.Response.RowsData.Should().HaveCount(2);
    endpoint.Response.PageNumber.Should().Be(1);
    endpoint.Response.PageSize.Should().Be(10);
    endpoint.Response.TotalCount.Should().Be(2);
  }

  [Fact]
  public async Task HandleAsync_ReturnsNotFound_WhenNoResults()
  {
    // Arrange
    var request = new ListClientRequest { Page = 1, PageSize = 10, SearchTerm = "test" };
    var result = Result<List<ClientDTO>>.NotFound();

    _mediator.Setup(m => m.Send(It.IsAny<ListClientQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);
    var endpoint = Factory.Create<ListClient>(_mediator.Object, _logger.Object);

    // Act
    await endpoint.HandleAsync(request, CancellationToken.None);

    // Assert
    endpoint.HttpContext.Response.StatusCode.Should().Be(404);
  }

  [Fact]
  public async Task HandleAsync_LogsError_WhenFailed()
  {
    // Arrange
    var request = new ListClientRequest { Page = 1, PageSize = 10, SearchTerm = "test" };
    var result = Result<List<ClientDTO>>.Error("Test Error");

    _mediator.Setup(m => m.Send(It.IsAny<ListClientQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);
    var endpoint = Factory.Create<ListClient>(_mediator.Object, _logger.Object);

    // Act
    await endpoint.HandleAsync(request, CancellationToken.None);

    // Assert
    _logger.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    endpoint.HttpContext.Response.StatusCode.Should().Be(404);
  }

  [Fact]
  public void Configure_SetsUniqueEndpointName()
  {
    // Arrange
    var endpoint = Factory.Create<ListClient>(_mediator.Object, _logger.Object);

    // Act
    endpoint.Configure();

    // Assert
    endpoint.Definition.EndpointType.Name.Should().StartWith("ListClient");
  }

  [Fact]
  public void Configure_SetsCorrectEndpointConfiguration()
  {
    // Arrange
    var endpoint = Factory.Create<ListClient>(_mediator.Object, _logger.Object);

    // Act
    endpoint.Configure();

    // Assert
    endpoint.Definition.Verbs.First().Should().Be(System.Net.WebRequestMethods.Http.Get);
    endpoint.Definition.Routes.First().Should().Contain(ListClientRequest.Route);
    endpoint.Definition.AllowAnyPermission.Should().BeFalse();
  }

  private static List<ClientDTO> GetFakeClients()
  {
    return new List<ClientDTO>
        {
            new ClientDTO(
                Guid.NewGuid(),
                "Test Client 1",
                "test1@example.com",
                "1",
                "123-456-7890",
                "123 Main St",
                "Anytown",
                "Anystate",
                "12345",
                "SA",
                ClientType.Regular,
                new TimeOnly(9, 0),
                ReferralSource.Website,
                new List<ClientPetDto>()),
            new ClientDTO(
                Guid.NewGuid(),
                "Test Client 2",
                "test2@example.com",
                "1",
                "123-456-7890",
                "123 Main St",
                "Anytown",
                "Anystate",
                "12345",
                "SA",
                ClientType.Regular,
                new TimeOnly(9, 0),
                ReferralSource.Website,
                new List<ClientPetDto>())
        };
  }
}
