using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.UnitTests.TestHelpers;

namespace FurryFriends.UnitTests.Core.PetWalkerAggregateTests.SpecificationTests;

public class ListPetWalkerSpecificationTests
{
  private IQueryable<PetWalker> _users;

  public ListPetWalkerSpecificationTests()
  {
    _users = PetWalkerHelpers.GetTestUsers().GetAwaiter().GetResult().AsQueryable();
  }
  [Fact]
  public async Task ReturnsAllUsers_WhenSearchStringIsEmpty()
  {
    var users = (await PetWalkerHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListPetWalkerSpecification(searchString: null);

    var query = spec.Evaluate(users);

    query.Count().Should().Be(5);
  }

  [Fact]
  public async Task FiltersUsersByFirstName_WhenSearchStringProvided()
  {
    var users = (await PetWalkerHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListPetWalkerSpecification("John");

    var query = spec.Evaluate(users);

    query.Should().HaveCount(1);
    query.First().Name.FirstName.Should().Be("John");
  }

  [Fact]
  public async Task FiltersUsersByEmail_WhenSearchStringMatchesEmail()
  {
    var users = (await PetWalkerHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListPetWalkerSpecification("jane.doe@example.com");

    var query = spec.Evaluate(users);

    query.Should().HaveCount(1);
    query.First().Email.EmailAddress.Should().Be("jane.doe@example.com");
  }

  [Theory]
  [InlineData(1, 2)]
  [InlineData(2, 2)]
  public async Task PaginatesResults_WhenPageParametersProvided(int pageNumber, int pageSize)
  {
    var users = (await PetWalkerHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListPetWalkerSpecification(null, pageNumber, pageSize);

    var query = spec.Evaluate(users);

    query.Count().Should().BeLessOrEqualTo(pageSize);
  }

  [Fact]
  public void ShouldNotReturnAnyUsersWhenSearchStringDoesNotExist()
  {
    var searchString = "NonExistentUser";
    var spec = new ListPetWalkerSpecification(searchString);

    var result = spec.Evaluate(_users);

    result.Should().BeEmpty();
  }

  [Theory]
  [InlineData("John")]
  [InlineData("Joh")]
  [InlineData("ohn")]
  [InlineData("jane.do")]
  public void ShouldReturnUsersWhenSearchStringDoesPartialMatch(string searchString)
  {

    var spec = new ListPetWalkerSpecification(searchString);

    var result = spec.Evaluate(_users);

    result.Count().Should().Be(1);
  }

  [Fact]
  public async Task OrdersUsersByFirstName()
  {
    var users = (await PetWalkerHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListPetWalkerSpecification(null);

    var query = spec.Evaluate(users).ToList();

    query[0].Name.FirstName.Should().Be("Alice");
    query[1].Name.FirstName.Should().Be("Bob");
    query[2].Name.FirstName.Should().Be("Charlie");
  }

}
