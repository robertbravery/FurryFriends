using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.Entities;
using Ardalis.Result;
using FurryFriends.Core.UserAggregate.Specifications;
using FurryFriends.UnitTests.TestHelpers;

namespace FurryFriends.UnitTests.Core.UserAggregate.Specification;

public class UserSpecificationTests
{
  private PhoneNumberValidator _validator;
  private Result<PhoneNumber> _phoneNumber;
  private List<User> _users;

  public UserSpecificationTests()
  {
    _validator = new PhoneNumberValidator();
    _phoneNumber = PhoneNumber.Create("027", "011", "1234567890", _validator);
    _users = UserHelpers.GetTestUsers();
  }

  [Fact]
  public void GetUserByEmailSpec_MatchesCorrectEmail()
  {
    // Arrange
    var targetEmail = "bob.wilson@example.com";
    var spec = new GetUserByEmailSpec(targetEmail);
    // Act
    var result = spec.Evaluate(_users);

    // Assert
    Assert.True(result.Count() == 1);
  }

  [Fact]
  public void GetUserByEmailSpec_DoesNotMatchDifferentEmail()
  {
    // Arrange
    var spec = new GetUserByEmailSpec("different@example.com");

    // Act
    var result = spec.Evaluate(_users);

    // Assert
    Assert.False(result == null);
  }
}
