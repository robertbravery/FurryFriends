using Ardalis.GuardClauses;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.Clients.Command.UpdateClient;
using FurryFriends.UseCases.Services.ClientService;
using Moq;

namespace FurryFriends.UnitTests.UseCase.ClientTest;

public class UpdateClientCommandHandlerTests
{
    private readonly Mock<IReadRepository<Client>> _mockClientRepository;
    private readonly Mock<IClientService> _mockClientService;
    private readonly UpdateClientCommandHandler _handler;
    private readonly CancellationToken _ct;

    public UpdateClientCommandHandlerTests()
    {
        _mockClientRepository = new Mock<IReadRepository<Client>>();
        _mockClientService = new Mock<IClientService>();
        _handler = new UpdateClientCommandHandler(_mockClientRepository.Object, _mockClientService.Object);
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ShouldUpdateClient_WhenValidDataProvided()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var existingClient = await CreateTestClientAsync(clientId);
        var command = CreateCommand(clientId);

        _mockClientRepository.Setup(r => r.GetByIdAsync(clientId, _ct))
            .ReturnsAsync(existingClient);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.Should().NotBeNull();
        result.Name.FullName.Should().Be("Updated Name");
        result.Email.EmailAddress.Should().Be("updated@example.com");
        _mockClientService.Verify(s => s.UpdateClientAsync(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenClientDoesNotExist()
    {
        // Arrange
        var command = new UpdateClientCommand { ClientId = Guid.NewGuid() };
        _mockClientRepository.Setup(r => r.GetByIdAsync(command.ClientId, _ct))
            .ReturnsAsync((Client)null!);

        // Act
        var act = () => _handler.Handle(command, _ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUpdateFails()
  {
    // Arrange
    var clientId = Guid.NewGuid();
    var existingClient = await CreateTestClientAsync(clientId);
    var command = CreateCommand(clientId);

    _mockClientRepository.Setup(r => r.GetByIdAsync(clientId, _ct))
        .ReturnsAsync(existingClient);
    _mockClientService.Setup(s => s.UpdateClientAsync(It.IsAny<Client>()))
        .ThrowsAsync(new Exception("Update failed"));

    // Act
    var act = () => _handler.Handle(command, _ct);

    // Assert
    await act.Should().ThrowAsync<Exception>()
        .WithMessage("Update failed");
  }

  private static UpdateClientCommand CreateCommand(Guid clientId)
  {
    return new UpdateClientCommand
    {
      ClientId = clientId,
      FirstName = "Updated",
      LastName = "Name",
      Email = "updated@example.com",
      CountryCode = "027",
      PhoneNumber = "555-0123",
      Street = "New Street",
      City = "New City",
      StateProvinceRegion = "New State",
      ZipCode = "54321",
      Country = "US",
      ClientType = ClientType.Premium,
      PreferredContactTime = new TimeOnly(10, 0),
      ReferralSource = "Updated Source"
    };
  }

  private static async Task<Client> CreateTestClientAsync(Guid id)
    {
        var client  = Client.Create(
            Name.Create("John", "Doe"),
            Email.Create("john.doe@example.com"),
            await PhoneNumber.Create("027","555-1234"),
            Address.Create("123 Main St",  "AnyCity", "CA", "USA", "12345"),
            ClientType.Regular);
        
        return client;
    }
}