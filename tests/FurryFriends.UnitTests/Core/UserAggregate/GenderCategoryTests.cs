using FurryFriends.Core.ValueObjects;
using static FurryFriends.Core.ValueObjects.GenderType;

namespace FurryFriends.UnitTests.Core.UserAggregate;

public class GenderCategoryTests
{
  [Fact]
  public void Create_GenderCategory_ReturnsGenderType()
  {
    var gender = GenderCategory.Male;
    var result = GenderType.Create(gender);

    result.Value.Should().BeOfType<GenderType>();
  }

  [Fact]
  public void Create_InvalidGender_ThrowsArgumentException()
  {
    var invalidGender = (GenderCategory)100; // Invalid enum value

    var genderResult = GenderType.Create(invalidGender);

    genderResult.IsSuccess.Should().BeFalse();
    genderResult.Errors.First().Should().Contain("Invalid gender");
  }

  [Fact]
  public void IsValidGender_ReturnsTrueForValidGender()
  {
    var validGender = GenderCategory.Male;
    bool isValid = GenderType.IsValidGender(validGender);

    isValid.Should().BeTrue();
  }

  [Fact]
  public void IsValidGender_ReturnsFalseForInvalidGender()
  {
    var invalidGender = (GenderCategory)100; // Invalid enum value
    bool isValid = GenderType.IsValidGender(invalidGender);

    isValid.Should().BeFalse();
  }
}
