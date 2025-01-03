using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.UserAggregate.Specifications;
using FurryFriends.UnitTests.TestHelpers;

namespace FurryFriends.UnitTests.Core.UserAggregate.Specification;

public class ListUserSpecificationTests
{
  private IQueryable<User> _users;

  public ListUserSpecificationTests()
  {
     _users =  UserHelpers.GetTestUsers().GetAwaiter().GetResult().AsQueryable();
  }
  [Fact]
  public async Task ReturnsAllUsers_WhenSearchStringIsEmpty()
  {
    var users = (await UserHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListUserSpecification(searchString: null);

    var query = spec.Evaluate(users);

    query.Count().Should().Be(5);
  }

  [Fact]
  public async Task FiltersUsersByFirstName_WhenSearchStringProvided()
  {
    var users = (await UserHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListUserSpecification("John");

    var query = spec.Evaluate(users);

    query.Should().HaveCount(1);
    query.First().Name.FirstName.Should().Be("John");
  }

  [Fact]
  public async Task FiltersUsersByEmail_WhenSearchStringMatchesEmail()
  {
    var users = (await UserHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListUserSpecification("jane.doe@example.com");

    var query = spec.Evaluate(users);

    query.Should().HaveCount(1);
    query.First().Email.EmailAddress.Should().Be("jane.doe@example.com");
  }

  [Theory]
  [InlineData(1, 2)]
  [InlineData(2, 2)]
  public async Task PaginatesResults_WhenPageParametersProvided(int pageNumber, int pageSize)
  {
    var users = (await UserHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListUserSpecification(null, pageNumber, pageSize);

    var query = spec.Evaluate(users);

    query.Count().Should().BeLessOrEqualTo(pageSize);
  }

  [Fact]
  public void ShouldNotReturnAnyUsersWhenSearchStringDoesNotExist()
  {
    var searchString = "NonExistentUser";
    var spec = new ListUserSpecification(searchString);

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
   
    var spec = new ListUserSpecification(searchString);

    var result = spec.Evaluate(_users);

    result.Count().Should().Be(1);
  }

  [Fact]
  public async Task OrdersUsersByFirstName()
  {
    var users = (await UserHelpers.GetTestUsers()).AsQueryable();
    var spec = new ListUserSpecification(null);

    var query = spec.Evaluate(users).ToList();

    query[0].Name.FirstName.Should().Be("Alice");
    query[1].Name.FirstName.Should().Be("Bob");
    query[2].Name.FirstName.Should().Be("Charlie");
  }

}
