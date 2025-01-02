using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UnitTests.Core.UserAggregate;

public class DateOfBirthTests
{
  [Fact]
  public void Create_ValidDate_ReturnsSuccess()
  {
    var date = new DateTime(1990, 5, 15);
    var result = DateOfBirth.Create(date);

    result.IsSuccess.Should().BeTrue();
    result.Value.Date.Should().Be(date);
  }

  [Fact]
  public void DateOfBirt_Younger_Than_18_ReturnsError()
  {

  }

  [Theory]
  [InlineData(2024, 13, 1)] // Invalid month
  [InlineData(2024, 5, 32)] // Invalid day
  [InlineData(2023, 2, 29)] // Invalid day for non-leap year
  public void DateOfBirth_InvalidDate_ThrowsException(int year, int month, int day) 
  {
    // Act & Assert
    Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(year, month, day)); 
  }

  [Fact]
  public void Create_LeapYear_ReturnsSuccess()
  {
    // Leap year check
    var leapYearDate = new DateTime(2024, 2, 29);
    var result = DateOfBirth.Create(leapYearDate);

    result.IsSuccess.Should().BeTrue();
    result.Value.Date.Should().Be(leapYearDate);
  }

 

  [Theory]
  [InlineData(70)]
  [InlineData(75)]
  [InlineData(80)]
  [InlineData(85)]
  public void Create_YearTooFarInThePast_ReturnsError(int years)
  {
    var invalidDate = DateTime.Now.AddYears(-years);
    var result = DateOfBirth.Create(invalidDate);

    result.IsSuccess.Should().BeFalse();
    result.Errors.First().Should().Contain("Date of birth is too old for a labour-type worker");
  }

  [Theory]
  [InlineData(1)]
  [InlineData(10)]
  [InlineData(100)]
  public void Create_YearInTheFuture_ReturnsError(int days)
  {
    var invalidDate = DateTime.Now.AddDays(days);
    var result = DateOfBirth.Create(invalidDate);

    result.IsSuccess.Should().BeFalse();
    result.Errors.First().Should().Contain("Date of birth cannot be in the future.");
  }
}

