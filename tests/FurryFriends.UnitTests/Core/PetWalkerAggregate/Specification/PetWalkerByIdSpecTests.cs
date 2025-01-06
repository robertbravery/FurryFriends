using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UnitTests.TestHelpers;

namespace FurryFriends.UnitTests.Core.PetWalkerAggregate.Specification;

public class UserSpecificationTests
{
  private PhoneNumberValidator _validator;
  private List<PetWalker> _users;

  public UserSpecificationTests()
  {
    _validator = new PhoneNumberValidator();
    _users = PetWalkerHelpers.GetTestUsers().GetAwaiter().GetResult();
  }

  [Fact]
  public void GetUserByEmailSpec_MatchesCorrectEmail()
  {
    // Arrange
    var targetEmail = "bob.wilson@example.com";
    var spec = new GetPetWalkerByEmailSpecification(targetEmail);
    // Act
    var result = spec.Evaluate(_users);

    // Assert
    result.Should().NotBeEmpty();
    result.Should().HaveCount(1);
    result.First().Email.EmailAddress.Should().Be(targetEmail);
  }

  [Fact]
  public void GetUserByEmailSpec_DoesNotMatchDifferentEmail()
  {
    // Arrange
    var spec = new GetPetWalkerByEmailSpecification("different@example.com");

    // Act
    var result = spec.Evaluate(_users);

    // Assert
    result.Should().BeEmpty();
  }
}
