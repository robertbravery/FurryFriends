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
    var canellationToken = new CancellationToken();

    await repository.AddAsync(Contributor, canellationToken);

    var newContributor = (await repository.ListAsync(canellationToken))
                    .FirstOrDefault();

    newContributor.Should().NotBeNull();
    newContributor?.Name.Should().BeEquivalentTo(testContributorName);
    newContributor?.Status.Should().BeEquivalentTo(testContributorStatus);
    newContributor?.Id.Should().BeGreaterThan(0);
  }
}
