using FurryFriends.Core.ValueObjects;
using static FurryFriends.Core.ValueObjects.GenderType;

namespace FurryFriends.UnitTests.Core.PetWalkerAggregateTests;

public class GenderCategoryTests
{
  [Fact]
  public void Create_GenderCategory_ReturnsGenderType()
  {
    var gender = GenderCategory.Male;
    var result = Create(gender);

    result.Value.Should().BeOfType<GenderType>();
  }

  [Fact]
  public void Create_InvalidGender_ThrowsArgumentException()
  {
    var invalidGender = (GenderCategory)100; // Invalid enum value

    var genderResult = Create(invalidGender);

    genderResult.IsSuccess.Should().BeFalse();
    genderResult.Errors.First().Should().Contain("Invalid gender");
  }

  [Fact]
  public void IsValidGender_ReturnsTrueForValidGender()
  {
    var validGender = GenderCategory.Male;
    var isValid = IsValidGender(validGender);

    isValid.Should().BeTrue();
  }

  [Fact]
  public void IsValidGender_ReturnsFalseForInvalidGender()
  {
    var invalidGender = (GenderCategory)100; // Invalid enum value
    var isValid = IsValidGender(invalidGender);

    isValid.Should().BeFalse();
  }
}
