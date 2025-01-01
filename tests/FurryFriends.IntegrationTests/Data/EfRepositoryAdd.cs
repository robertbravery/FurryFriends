using FluentAssertions;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
  [Fact]
  public async Task AddsContributorAndSetsId()
  {
    var testContributorName = Name.Create("Jane", "Doe", new NameValidator()).Value; //"test Contributor";
    var testContributorStatus = ContributorStatus.NotSet;
    var repository = GetRepository();
    var Contributor = new Contributor(testContributorName);

    await repository.AddAsync(Contributor);

    var newContributor = (await repository.ListAsync())
                    .FirstOrDefault();

    newContributor.Should().NotBeNull();
    newContributor?.Name.Should().BeEquivalentTo(testContributorName);
    newContributor?.Status.Should().BeEquivalentTo(testContributorStatus);
    newContributor?.Id.Should().BeGreaterThan(0);
  }
}
