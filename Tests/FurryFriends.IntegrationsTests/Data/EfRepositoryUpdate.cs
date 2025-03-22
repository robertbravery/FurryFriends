using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.IntegrationsTests.Data;

public class EfRepositoryUpdate : BaseEfRepoTestFixture
{
  [Fact]
  public async Task UpdatesItemAfterAddingIt()
  {
    // add a Contributor
    var repository = GetRepository();
    var firstName = "John";
    var lastName = "Doe";
    var initialName = Name.Create(firstName, lastName);
    var Contributor = new Contributor(initialName);
    var canellationToken = new CancellationToken();


    await repository.AddAsync(Contributor, canellationToken);

    // detach the item so we get a different instance
    _dbContext.Entry(Contributor).State = EntityState.Detached;

    // fetch the item and update its title
    var newContributor = (await repository.ListAsync(canellationToken))
        .FirstOrDefault(Contributor => Contributor.Name == initialName);
    if (newContributor == null)
    {
      Assert.NotNull(newContributor);
      return;
    }

    newContributor.Should().NotBeSameAs(Contributor);

    var newFirstName = "Jane";
    var newLastName = "Doe";
    var newName = Name.Create(newFirstName, newLastName);
    newContributor.UpdateName(newName);

    // Update the item
    await repository.UpdateAsync(newContributor, canellationToken);

    // Fetch the updated item
    var updatedItem = (await repository.ListAsync(canellationToken))
        .FirstOrDefault(Contributor => Contributor.Name == newName);

    updatedItem.Should().NotBeNull();
    updatedItem?.Name.Should().NotBe(Contributor.Name);
    updatedItem?.Name.Should().Be(newName);
    updatedItem?.Status.Should().Be(Contributor.Status);
  }
}
