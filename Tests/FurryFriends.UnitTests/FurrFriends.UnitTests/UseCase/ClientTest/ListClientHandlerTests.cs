using Ardalis.Result;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.UnitTests.TestHelpers;
using FurryFriends.UseCases.Domain.Clients.Query.ListClients;
using FurryFriends.UseCases.Services.ClientService;
using Moq;

namespace FurryFriends.UnitTests.UseCase.ClientTest;
public class ListClientHandlerTests
{
    private readonly Mock<IClientService> _mockClientService;
    private readonly ListClientHandler _handler;
    private readonly CancellationToken _ct;

    public ListClientHandlerTests()
    {
        _mockClientService = new Mock<IClientService>();
        _handler = new ListClientHandler(_mockClientService.Object);
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ShouldReturnClientList_WhenClientsExist()
    {
        // Arrange
        var clients = ClientTestHelpers.CreateTestClients(2);
        var query = new ListClientQuery("", 1, 10);

        _mockClientService.Setup(s => s.ListClientsAsync(query.SearchTerm, query.Page, query.PageSize, _ct))
            .ReturnsAsync(Result<IEnumerable<Client>>.Success(clients));

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenNoClientsExist()
    {
        // Arrange
        var query = new ListClientQuery("", 1, 10);

        _mockClientService.Setup(s => s.ListClientsAsync(query.SearchTerm, query.Page, query.PageSize, _ct))
            .ReturnsAsync(Result.NotFound());

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }
}