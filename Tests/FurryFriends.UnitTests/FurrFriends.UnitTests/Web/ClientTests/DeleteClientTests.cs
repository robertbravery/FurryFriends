using System.Net;
using Ardalis.Result;
using FastEndpoints;
using FurryFriends.UseCases.Domain.Clients.Command.DeleteClient;
using FurryFriends.Web.Endpoints.ClientEndpoints.Delete;
using Moq;

namespace FurryFriends.UnitTests.Web.ClientTests;
public class DeleteClientTests
{
  private readonly Mock<IMediator> _mockMediator;
  private readonly DeleteClient _handler;

  public DeleteClientTests()
  {
    _mockMediator = new Mock<IMediator>();
    _handler = Factory.Create<DeleteClient>(_mockMediator.Object);

  }

  [Fact]
  public async Task HandleAsync_ShouldReturnNoContent_WhenClientIsDeletedSuccessfully()
  {
    var clientId = Guid.NewGuid();
    var request = new DeleteClientRequest { ClientId = clientId };

    _mockMediator
        .Setup(m => m.Send(It.Is<DeleteClientCommand>(cmd => cmd.ClientId == clientId), It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result.Success());

    await _handler.HandleAsync(request, CancellationToken.None);

    _handler.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnNotFound_WhenClientIsNotFound()
  {
    var clientId = Guid.NewGuid();
    var request = new DeleteClientRequest { ClientId = clientId };

    _mockMediator
        .Setup(m => m.Send(It.Is<DeleteClientCommand>(cmd => cmd.ClientId == clientId), It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result.NotFound());

    await _handler.HandleAsync(request, CancellationToken.None);

    _handler.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnBadRequest_WhenUnexpectedFailureOccurs()
  {
    var clientId = Guid.NewGuid();
    var request = new DeleteClientRequest { ClientId = clientId };

    _mockMediator
        .Setup(m => m.Send(It.Is<DeleteClientCommand>(cmd => cmd.ClientId == clientId), It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result.Error("Unexpected error"));

    await _handler.HandleAsync(request, CancellationToken.None);

    _handler.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
  }
}

