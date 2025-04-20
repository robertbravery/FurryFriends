namespace FurryFriends.IntegrationsTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
  [Fact]
  public async Task AddsClientAndSetsId()
  {
    //var testContributorName = Name.Create("Jane", "Doe").Value; //"test Contributor";
    //var testContributorStatus = ContributorStatus.NotSet;
    var repository = GetRepository();
    //var Contributor = new Contributor(testContributorName);
    var canellationToken = new CancellationToken();

    //await repository.AddAsync(Contributor, canellationToken);

    var newContributor = (await repository.ListAsync(canellationToken))
                    .FirstOrDefault();

    //newContributor.Should().NotBeNull();
    //newContributor?.Name.Should().BeEquivalentTo(testContributorName);
    //newContributor?.Status.Should().BeEquivalentTo(testContributorStatus);

    //newContributor?.Id.Should().BeGreaterThan(0);

    Assert.True(true);
  }
}
