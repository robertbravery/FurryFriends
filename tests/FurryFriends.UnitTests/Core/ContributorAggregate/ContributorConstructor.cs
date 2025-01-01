using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UnitTests.Core.ContributorAggregate;

public class ContributorConstructor
{
  private readonly Name _testName = Name.Create("name", "name", new NameValidator()).Value; //"test name";
  private Contributor? _testContributor;

  private Contributor CreateContributor()
  {
    return new Contributor(_testName);
  }

  [Fact]
  public void InitializesName()
  {
    _testContributor = CreateContributor();

    Assert.Equal(_testName, _testContributor.Name);
  }
}
