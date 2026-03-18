using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.UseCases.Timeslots.CustomTimeRequest;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CustomTimeRequestEntity = FurryFriends.Core.TimeslotAggregate.CustomTimeRequest;

namespace FurryFriends.UnitTests.UseCase.Timeslots.CustomTimeRequest;

public class RequestCustomTimeTests
{
    private readonly Mock<IRepository<PetWalker>> _petWalkerRepositoryMock;
    private readonly Mock<IRepository<CustomTimeRequestEntity>> _customTimeRequestRepositoryMock;
    private readonly Mock<ILogger<RequestCustomTimeHandler>> _loggerMock;
    private readonly RequestCustomTimeHandler _handler;

    public RequestCustomTimeTests()
    {
        _petWalkerRepositoryMock = new Mock<IRepository<PetWalker>>();
        _customTimeRequestRepositoryMock = new Mock<IRepository<CustomTimeRequestEntity>>();
        _loggerMock = new Mock<ILogger<RequestCustomTimeHandler>>();

        _handler = new RequestCustomTimeHandler(
            _petWalkerRepositoryMock.Object,
            _customTimeRequestRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static PetWalker CreateTestPetWalker()
    {
        // Use reflection to create a PetWalker instance for testing
        var name = FurryFriends.Core.ValueObjects.Name.Create("John", "Doe").Value;
        var email = FurryFriends.Core.ValueObjects.Email.Create("john@example.com").Value;
        var address = FurryFriends.Core.ValueObjects.Address.Create("123 Main St", "Johannesburg", "Gauteng", "2001", "South Africa").Value;
        
        var petWalker = PetWalker.Create(name, email, null!, address);
        return petWalker;
    }

    [Fact]
    public async Task Handle_PetWalkerNotFound_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var duration = 30;
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        _petWalkerRepositoryMock
            .Setup(x => x.GetByIdAsync(petWalkerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PetWalker?)null);

        var command = new RequestCustomTimeCommand(
            petWalkerId,
            clientId,
            requestedDate,
            preferredStartTime,
            duration,
            clientAddress,
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Petwalker not found", result.Errors.First());
    }

    [Fact]
    public async Task Handle_DuplicatePendingRequest_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var duration = 30;
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        var petWalker = CreateTestPetWalker();

        var existingRequest = CustomTimeRequestEntity.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            duration,
            clientAddress).Value;

        _petWalkerRepositoryMock
            .Setup(x => x.GetByIdAsync(petWalkerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalker);

        _customTimeRequestRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<PendingCustomTimeRequestByClientAndPetWalkerSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CustomTimeRequestEntity> { existingRequest });

        var command = new RequestCustomTimeCommand(
            petWalkerId,
            clientId,
            requestedDate,
            preferredStartTime,
            duration,
            clientAddress,
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("pending custom time request", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var duration = 30;
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        var petWalker = CreateTestPetWalker();

        _petWalkerRepositoryMock
            .Setup(x => x.GetByIdAsync(petWalkerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalker);

        _customTimeRequestRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<PendingCustomTimeRequestByClientAndPetWalkerSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CustomTimeRequestEntity>());

        _customTimeRequestRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<CustomTimeRequestEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomTimeRequestEntity r, CancellationToken _) => r);

        var command = new RequestCustomTimeCommand(
            petWalkerId,
            clientId,
            requestedDate,
            preferredStartTime,
            duration,
            clientAddress,
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(clientId, result.Value.ClientId);
        Assert.Equal(petWalkerId, result.Value.PetWalkerId);
        Assert.Equal("Pending", result.Value.Status);
    }
}
