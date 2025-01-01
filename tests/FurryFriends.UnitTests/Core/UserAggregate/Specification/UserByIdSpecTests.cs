using FurryFriends.Core.ValueObjects;
using Ardalis.Result;
using FurryFriends.Core.UserAggregate.Specifications;
using FurryFriends.UnitTests.TestHelpers;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UnitTests.Core.UserAggregate.Specification;

public class UserSpecificationTests
{
  private PhoneNumberValidator _validator;
  private List<User> _users;

  public UserSpecificationTests()
  {
    _validator = new PhoneNumberValidator();
    _users = UserHelpers.GetTestUsers().GetAwaiter().GetResult();
  }

  [Fact]
  public void GetUserByEmailSpec_MatchesCorrectEmail()
  {
    // Arrange
    var targetEmail = "bob.wilson@example.com";
    var spec = new GetUserByEmailSpecification(targetEmail);
    // Act
    var result = spec.Evaluate(_users);

    // Assert
    result.Should().NotBeEmpty();
    result.Should().HaveCount(1);
    result.First().Email.Should().Be(targetEmail);
  }

  [Fact]
  public void GetUserByEmailSpec_DoesNotMatchDifferentEmail()
  {
    // Arrange
    var spec = new GetUserByEmailSpecification("different@example.com");

    // Act
    var result = spec.Evaluate(_users);

    // Assert
    result.Should().BeEmpty();
  }
}
