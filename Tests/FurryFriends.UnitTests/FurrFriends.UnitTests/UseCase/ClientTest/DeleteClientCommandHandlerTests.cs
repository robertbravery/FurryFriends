using FurryFriends.Core.ClientAggregate;
using FurryFriends.UseCases.Domain.Clients.Command.DeleteClient;
using Moq;

namespace FurryFriends.UnitTests.UseCase.ClientTest;

public class DeleteClientCommandHandlerTests
{
  private readonly Mock<IRepository<Client>> _mockClientRepository;
  private readonly DeleteClientCommandHandler _handler;
  private readonly CancellationToken _ct;

  public DeleteClientCommandHandlerTests()
  {
    _mockClientRepository = new Mock<IRepository<Client>>();
    _handler = new DeleteClientCommandHandler(_mockClientRepository.Object);
    _ct = CancellationToken.None;
  }

  [Fact]
  public async Task Handle_ShouldReturnSuccess_WhenClientExists()
  {
    // Arrange
    var clientId = Guid.NewGuid();
    var client = CreateTestClient(clientId);

    _mockClientRepository
        .Setup(repo => repo.GetByIdAsync(clientId, _ct))
        .ReturnsAsync(client);

    _mockClientRepository
        .Setup(repo => repo.DeleteAsync(client, _ct))
        .Returns(Task.CompletedTask);

    var command = new DeleteClientCommand(clientId);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();

    _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId, _ct), Times.Once);
    _mockClientRepository.Verify(repo => repo.DeleteAsync(client, _ct), Times.Once);
  }

  [Fact]
  public async Task Handle_ShouldReturnNotFound_WhenClientDoesNotExist()
  {
    // Arrange
    var clientId = Guid.NewGuid();

    _mockClientRepository
        .Setup(repo => repo.GetByIdAsync(clientId, _ct))
        .ReturnsAsync((Client)null!);

    var command = new DeleteClientCommand(clientId);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeFalse();

    _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId, _ct), Times.Once);
    _mockClientRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Client>(), _ct), Times.Never);
  }

  [Fact]
  public async Task Handle_ShouldThrowException_WhenDeleteFails()
  {
    // Arrange
    var clientId = Guid.NewGuid();
    var client = CreateTestClient(clientId);

    _mockClientRepository
        .Setup(repo => repo.GetByIdAsync(clientId, _ct))
        .ReturnsAsync(client);

    _mockClientRepository
        .Setup(repo => repo.DeleteAsync(client, _ct))
        .ThrowsAsync(new Exception("Database error"));

    var command = new DeleteClientCommand(clientId);

    // Act
    var act = () => _handler.Handle(command, CancellationToken.None);

    // Assert
    await act.Should().ThrowAsync<Exception>()
        .WithMessage("Database error");

    _mockClientRepository.Verify(repo => repo.GetByIdAsync(clientId, _ct), Times.Once);
    _mockClientRepository.Verify(repo => repo.DeleteAsync(client, _ct), Times.Once);
  }

  private static Client CreateTestClient(Guid clientId)
  {
    var client = new Client
    {
      Id = clientId
    };
    return client;
  }
}
