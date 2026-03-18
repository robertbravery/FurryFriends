using Bogus;
using FurryFriends.Core.Entities;
using FurryFriends.Core.Enums;

namespace FurryFriends.UnitTests.Core.TimeslotAggregate;

public class CustomTimeRequestTests
{
    private readonly Faker _faker;

    public CustomTimeRequestTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_CustomTimeRequest_WithValidData_ReturnsValidRequest()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CustomTimeRequest>();
        result.Value.ClientId.Should().Be(clientId);
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.RequestedDate.Should().Be(requestedDate);
        result.Value.PreferredStartTime.Should().Be(preferredStartTime);
        result.Value.PreferredDurationMinutes.Should().Be(preferredDurationMinutes);
        result.Value.ClientAddress.Should().Be(clientAddress);
        result.Value.Status.Should().Be(CustomTimeRequestStatus.Pending);
    }

    [Fact]
    public void Create_CustomTimeRequest_PreferredStartTimePlusDuration_EqualsPreferredEndTime()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(14, 0);
        var preferredDurationMinutes = 45;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PreferredEndTime.Should().Be(new TimeOnly(14, 45));
    }

    [Fact]
    public void Create_CustomTimeRequest_WithTodayDate_ReturnsValidRequest()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today);
        var preferredStartTime = new TimeOnly(DateTime.Now.Hour + 1, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_CustomTimeRequest_WithFutureDate_ReturnsValidRequest()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_CustomTimeRequest_WithPastDate_ReturnsError()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_CustomTimeRequest_WithDuration30_ReturnsValidRequest()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PreferredDurationMinutes.Should().Be(30);
    }

    [Fact]
    public void Create_CustomTimeRequest_WithDuration45_ReturnsValidRequest()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 45;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PreferredDurationMinutes.Should().Be(45);
    }

    [Fact]
    public void Create_CustomTimeRequest_WithDurationLessThan30_ReturnsError()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 29;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_CustomTimeRequest_WithDurationGreaterThan45_ReturnsError()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 46;
        var clientAddress = _faker.Address.StreetAddress();

        // Act
        var result = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Accept_Request_ChangesStatusToAccepted()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        var request = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress).Value;

        // Act
        request.Accept();

        // Assert
        request.Status.Should().Be(CustomTimeRequestStatus.Accepted);
    }

    [Fact]
    public void Decline_Request_ChangesStatusToDeclined()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();
        var reason = "Not available on that date";

        var request = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress).Value;

        // Act
        request.Decline(reason);

        // Assert
        request.Status.Should().Be(CustomTimeRequestStatus.Declined);
        request.ResponseReason.Should().Be(reason);
    }

    [Fact]
    public void CounterOffer_Request_ChangesStatusToCounterOffered()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();
        var counterOfferedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var counterOfferedTime = new TimeOnly(14, 0);
        var reason = "Better availability on this date";

        var request = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress).Value;

        // Act
        request.CounterOffer(counterOfferedDate, counterOfferedTime, reason);

        // Assert
        request.Status.Should().Be(CustomTimeRequestStatus.CounterOffered);
        request.CounterOfferedDate.Should().Be(counterOfferedDate);
        request.CounterOfferedTime.Should().Be(counterOfferedTime);
        request.ResponseReason.Should().Be(reason);
    }

    [Fact]
    public void Expire_Request_ChangesStatusToExpired()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        var request = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress).Value;

        // Act
        request.Expire();

        // Assert
        request.Status.Should().Be(CustomTimeRequestStatus.Expired);
    }

    [Fact]
    public void Accept_Request_FromNonPendingStatus_ReturnsError()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        var request = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress).Value;

        // First accept the request
        request.Accept();

        // Act - Try to accept again
        var result = request.Accept();

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Decline_Request_FromAcceptedStatus_ReturnsError()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var preferredDurationMinutes = 30;
        var clientAddress = _faker.Address.StreetAddress();

        var request = CustomTimeRequest.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress).Value;

        // First accept the request
        request.Accept();

        // Act - Try to decline an accepted request
        var result = request.Decline("Some reason");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
