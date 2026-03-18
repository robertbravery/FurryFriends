using FurryFriends.UseCases.Timeslots.Booking;
using Xunit;

namespace FurryFriends.UseCases.Timeslots.Booking.Tests;

public class TravelBufferCalculationTests
{
    private readonly TravelBufferCalculator _calculator;

    public TravelBufferCalculationTests()
    {
        _calculator = new TravelBufferCalculator();
    }

    [Fact]
    public void CalculateBufferMinutes_SameAddress_ReturnsZero()
    {
        // Arrange
        var address = "123 Main St, Johannesburg, Gauteng, 2001";

        // Act
        var result = _calculator.CalculateBufferMinutes(address, address);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateBufferMinutes_SimilarSuburb_ReturnsZero()
    {
        // Arrange
        var address1 = "123 Main St, Sandton, Gauteng, 2196";
        var address2 = "456 Oak Ave, Sandton, Gauteng, 2196";

        // Act
        var result = _calculator.CalculateBufferMinutes(address1, address2);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateBufferMinutes_DifferentAddress_ReturnsPositiveBuffer()
    {
        // Arrange
        var address1 = "123 Main St, Johannesburg, Gauteng, 2001";
        var address2 = "456 Oak Ave, Cape Town, Western Cape, 8001";

        // Act
        var result = _calculator.CalculateBufferMinutes(address1, address2);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void CalculateBufferMinutes_EmptyOriginAddress_ReturnsDefault()
    {
        // Arrange
        var emptyAddress = "";
        var validAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        // Act
        var result = _calculator.CalculateBufferMinutes(emptyAddress, validAddress);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void CalculateBufferMinutes_EmptyDestinationAddress_ReturnsDefault()
    {
        // Arrange
        var validAddress = "123 Main St, Johannesburg, Gauteng, 2001";
        var emptyAddress = "";

        // Act
        var result = _calculator.CalculateBufferMinutes(validAddress, emptyAddress);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void CalculateBufferMinutes_NullOriginAddress_ReturnsDefault()
    {
        // Arrange
        string? nullAddress = null;
        var validAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        // Act
        var result = _calculator.CalculateBufferMinutes(nullAddress!, validAddress);

        // Assert
        Assert.True(result > 0);
    }
}
