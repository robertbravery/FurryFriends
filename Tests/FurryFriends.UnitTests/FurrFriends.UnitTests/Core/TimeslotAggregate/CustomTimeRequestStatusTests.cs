using Bogus;
using FurryFriends.Core.Enums;

namespace FurryFriends.UnitTests.Core.TimeslotAggregate;

public class CustomTimeRequestStatusTests
{
    private readonly Faker _faker;

    public CustomTimeRequestStatusTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void CustomTimeRequestStatus_Pending_HasCorrectValue()
    {
        // Arrange & Act
        var status = CustomTimeRequestStatus.Pending;

        // Assert
        status.Should().Be((CustomTimeRequestStatus)0);
    }

    [Fact]
    public void CustomTimeRequestStatus_Accepted_HasCorrectValue()
    {
        // Arrange & Act
        var status = CustomTimeRequestStatus.Accepted;

        // Assert
        status.Should().Be((CustomTimeRequestStatus)1);
    }

    [Fact]
    public void CustomTimeRequestStatus_Declined_HasCorrectValue()
    {
        // Arrange & Act
        var status = CustomTimeRequestStatus.Declined;

        // Assert
        status.Should().Be((CustomTimeRequestStatus)2);
    }

    [Fact]
    public void CustomTimeRequestStatus_CounterOffered_HasCorrectValue()
    {
        // Arrange & Act
        var status = CustomTimeRequestStatus.CounterOffered;

        // Assert
        status.Should().Be((CustomTimeRequestStatus)3);
    }

    [Fact]
    public void CustomTimeRequestStatus_Expired_HasCorrectValue()
    {
        // Arrange & Act
        var status = CustomTimeRequestStatus.Expired;

        // Assert
        status.Should().Be((CustomTimeRequestStatus)4);
    }

    [Fact]
    public void CustomTimeRequestStatus_AllEnumValues_AreDefined()
    {
        // Arrange
        var expectedValues = new[]
        {
            CustomTimeRequestStatus.Pending,
            CustomTimeRequestStatus.Accepted,
            CustomTimeRequestStatus.Declined,
            CustomTimeRequestStatus.CounterOffered,
            CustomTimeRequestStatus.Expired
        };

        // Act & Assert
        Enum.GetValues<CustomTimeRequestStatus>().Should().BeEquivalentTo(expectedValues);
    }

    [Fact]
    public void CustomTimeRequestStatus_ConvertFromInt_ReturnsCorrectValue()
    {
        // Arrange
        var intValue = 1;

        // Act
        var status = (CustomTimeRequestStatus)intValue;

        // Assert
        status.Should().Be(CustomTimeRequestStatus.Accepted);
    }

    [Fact]
    public void CustomTimeRequestStatus_ConvertToInt_ReturnsCorrectValue()
    {
        // Arrange
        var status = CustomTimeRequestStatus.Pending;

        // Act
        var intValue = (int)status;

        // Assert
        intValue.Should().Be(0);
    }

    [Fact]
    public void CustomTimeRequestStatus_Parse_ReturnsCorrectValue()
    {
        // Arrange
        var stringValue = "Declined";

        // Act
        var status = Enum.Parse<CustomTimeRequestStatus>(stringValue);

        // Assert
        status.Should().Be(CustomTimeRequestStatus.Declined);
    }

    [Fact]
    public void CustomTimeRequestStatus_TryParse_ReturnsTrue_ForValidValue()
    {
        // Arrange
        var stringValue = "CounterOffered";

        // Act
        var result = Enum.TryParse<CustomTimeRequestStatus>(stringValue, out var status);

        // Assert
        result.Should().BeTrue();
        status.Should().Be(CustomTimeRequestStatus.CounterOffered);
    }

    [Fact]
    public void CustomTimeRequestStatus_TryParse_ReturnsFalse_ForInvalidValue()
    {
        // Arrange
        var stringValue = "InvalidStatus";

        // Act
        var result = Enum.TryParse<CustomTimeRequestStatus>(stringValue, out var status);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CustomTimeRequestStatus_ToString_ReturnsCorrectString()
    {
        // Arrange
        var status = CustomTimeRequestStatus.Expired;

        // Act
        var result = status.ToString();

        // Assert
        result.Should().Be("Expired");
    }
}
