using Bogus;
using FurryFriends.Core.Entities;
using FurryFriends.Core.Enums;

namespace FurryFriends.UnitTests.Core.TimeslotAggregate;

public class TravelBufferTests
{
    private readonly Faker _faker;

    public TravelBufferTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_TravelBuffer_WithValidData_ReturnsValidTravelBuffer()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 15;
        var startTime = DateTime.Now.AddHours(1);
        var endTime = DateTime.Now.AddHours(1).AddMinutes(15);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<TravelBuffer>();
        result.Value.BookingId.Should().Be(bookingId);
        result.Value.OriginAddress.Should().Be(originAddress);
        result.Value.DestinationAddress.Should().Be(destinationAddress);
        result.Value.BufferDurationMinutes.Should().Be(bufferDurationMinutes);
        result.Value.StartTime.Should().Be(startTime);
        result.Value.EndTime.Should().Be(endTime);
    }

    [Fact]
    public void Create_TravelBuffer_EndTimeGreaterThanStartTime_ReturnsValidTravelBuffer()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 30;
        var startTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 1, 10, 30, 0);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EndTime.Should().BeAfter(result.Value.StartTime);
    }

    [Fact]
    public void Create_TravelBuffer_WithBufferDurationMinutes1_ReturnsValidTravelBuffer()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 1;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BufferDurationMinutes.Should().Be(1);
    }

    [Fact]
    public void Create_TravelBuffer_WithBufferDurationMinutes60_ReturnsValidTravelBuffer()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 60;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationDescription: destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BufferDurationMinutes.Should().Be(60);
    }

    [Fact]
    public void Create_TravelBuffer_WithBufferDurationMinutes0_ReturnsError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 0;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_TravelBuffer_WithNegativeBufferDurationMinutes_ReturnsError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = -5;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_TravelBuffer_WithPreviousBooking_ReturnsValidTravelBuffer()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var previousBookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 15;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime, previousBookingId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PreviousBookingId.Should().Be(previousBookingId);
    }

    [Fact]
    public void Create_TravelBuffer_WithoutPreviousBooking_ReturnsValidTravelBuffer()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        Guid? previousBookingId = null;
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 15;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime, previousBookingId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PreviousBookingId.Should().BeNull();
    }

    [Fact]
    public void Create_TravelBuffer_MaxAddressLength_ReturnsValidTravelBuffer()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = new string('A', 500); // Max 500 characters
        var destinationAddress = new string('B', 500);
        var bufferDurationMinutes = 15;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_TravelBuffer_ExceedsMaxAddressLength_ReturnsError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = new string('A', 501); // Exceeds 500 characters
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 15;
        var startTime = DateTime.Now.AddHours(1);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_TravelBuffer_StartTimePlusDuration_EqualsEndTime()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var originAddress = _faker.Address.StreetAddress();
        var destinationAddress = _faker.Address.StreetAddress();
        var bufferDurationMinutes = 45;
        var startTime = new DateTime(2024, 1, 1, 10, 0, 0);

        // Act
        var result = TravelBuffer.Create(bookingId, originAddress, destinationAddress, bufferDurationMinutes, startTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EndTime.Should().Be(new DateTime(2024, 1, 1, 10, 45, 0));
    }
}
