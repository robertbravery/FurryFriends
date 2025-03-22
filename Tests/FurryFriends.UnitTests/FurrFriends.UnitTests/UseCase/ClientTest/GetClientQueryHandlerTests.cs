using Ardalis.Result;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.UnitTests.TestHelpers;
using FurryFriends.UseCases.Domain.Clients.Query.GetClient;
using FurryFriends.UseCases.Services.ClientService;
using Moq;

namespace FurryFriends.UnitTests.UseCase.ClientTest;

public class GetClientQueryHandlerTests
{
    private readonly Mock<IClientService> _mockClientService;
    private readonly GetClientQueryHandler _handler;
    private readonly CancellationToken _ct;

    public GetClientQueryHandlerTests()
    {
        _mockClientService = new Mock<IClientService>();
        _handler = new GetClientQueryHandler(_mockClientService.Object);
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ShouldReturnClientDTO_WhenClientExists()
    {
        // Arrange
        var email = "john.doe@example.com";
        var client = await ClientTestHelpers.CreateTestClientAsync();
        var query = new GetClientQuery(email);

        _mockClientService.Setup(s => s.GetClientAsync(email, _ct))
            .ReturnsAsync(Result.Success(client));

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(email);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var query = new GetClientQuery(email);

        _mockClientService.Setup(s => s.GetClientAsync(email, _ct))
            .ReturnsAsync(Result.NotFound());

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    private static Client CreateTestClient()
    {
        return new Client
        {
            Id = Guid.NewGuid(),
            // Set other required properties
        };
    }
}