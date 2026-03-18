using Bogus;
using FurryFriends.Core.Enums;

namespace FurryFriends.UnitTests.Core.TimeslotAggregate;

public class TimeslotStatusTests
{
    private readonly Faker _faker;

    public TimeslotStatusTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void TimeslotStatus_Available_HasCorrectValue()
    {
        // Arrange & Act
        var status = TimeslotStatus.Available;

        // Assert
        status.Should().Be((TimeslotStatus)0);
    }

    [Fact]
    public void TimeslotStatus_Booked_HasCorrectValue()
    {
        // Arrange & Act
        var status = TimeslotStatus.Booked;

        // Assert
        status.Should().Be((TimeslotStatus)1);
    }

    [Fact]
    public void TimeslotStatus_Unavailable_HasCorrectValue()
    {
        // Arrange & Act
        var status = TimeslotStatus.Unavailable;

        // Assert
        status.Should().Be((TimeslotStatus)2);
    }

    [Fact]
    public void TimeslotStatus_Cancelled_HasCorrectValue()
    {
        // Arrange & Act
        var status = TimeslotStatus.Cancelled;

        // Assert
        status.Should().Be((TimeslotStatus)3);
    }

    [Fact]
    public void TimeslotStatus_AllEnumValues_AreDefined()
    {
        // Arrange
        var expectedValues = new[]
        {
            TimeslotStatus.Available,
            TimeslotStatus.Booked,
            TimeslotStatus.Unavailable,
            TimeslotStatus.Cancelled
        };

        // Act & Assert
        Enum.GetValues<TimeslotStatus>().Should().BeEquivalentTo(expectedValues);
    }

    [Fact]
    public void TimeslotStatus_ConvertFromInt_ReturnsCorrectValue()
    {
        // Arrange
        var intValue = 1;

        // Act
        var status = (TimeslotStatus)intValue;

        // Assert
        status.Should().Be(TimeslotStatus.Booked);
    }

    [Fact]
    public void TimeslotStatus_ConvertToInt_ReturnsCorrectValue()
    {
        // Arrange
        var status = TimeslotStatus.Available;

        // Act
        var intValue = (int)status;

        // Assert
        intValue.Should().Be(0);
    }

    [Fact]
    public void TimeslotStatus_Parse_ReturnsCorrectValue()
    {
        // Arrange
        var stringValue = "Booked";

        // Act
        var status = Enum.Parse<TimeslotStatus>(stringValue);

        // Assert
        status.Should().Be(TimeslotStatus.Booked);
    }

    [Fact]
    public void TimeslotStatus_TryParse_ReturnsTrue_ForValidValue()
    {
        // Arrange
        var stringValue = "Available";

        // Act
        var result = Enum.TryParse<TimeslotStatus>(stringValue, out var status);

        // Assert
        result.Should().BeTrue();
        status.Should().Be(TimeslotStatus.Available);
    }

    [Fact]
    public void TimeslotStatus_TryParse_ReturnsFalse_ForInvalidValue()
    {
        // Arrange
        var stringValue = "InvalidStatus";

        // Act
        var result = Enum.TryParse<TimeslotStatus>(stringValue, out var status);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TimeslotStatus_ToString_ReturnsCorrectString()
    {
        // Arrange
        var status = TimeslotStatus.Unavailable;

        // Act
        var result = status.ToString();

        // Assert
        result.Should().Be("Unavailable");
    }
}
