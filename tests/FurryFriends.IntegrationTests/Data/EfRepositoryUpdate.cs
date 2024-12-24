using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.IntegrationTests.Data;

public class EfRepositoryUpdate : BaseEfRepoTestFixture
{
  [Fact]
  public async Task UpdatesItemAfterAddingIt()
  {
    // add a Contributor
    var repository = GetRepository();
    var firstName = "John";
    var lastName = "Doe";
    var initialName = Name.Create(firstName, lastName, firstName+" "+lastName, new NameValidator());
    var Contributor = new Contributor(initialName);

    await repository.AddAsync(Contributor);

    // detach the item so we get a different instance
    _dbContext.Entry(Contributor).State = EntityState.Detached;

    // fetch the item and update its title
    var newContributor = (await repository.ListAsync())
        .FirstOrDefault(Contributor => Contributor.Name == initialName);
    if (newContributor == null)
    {
      Assert.NotNull(newContributor);
      return;
    }
    Assert.NotSame(Contributor, newContributor);
    var newFirstName = "Jane";
    var newLastName = "Doe";
    var newName = Name.Create(newFirstName, newLastName,  firstName+" "+lastName, new NameValidator());
    newContributor.UpdateName(newName);

    // Update the item
    await repository.UpdateAsync(newContributor);

    // Fetch the updated item
    var updatedItem = (await repository.ListAsync())
        .FirstOrDefault(Contributor => Contributor.Name == newName);

    Assert.NotNull(updatedItem);
    Assert.NotEqual(Contributor.Name, updatedItem?.Name);
    Assert.Equal(Contributor.Status, updatedItem?.Status);
    Assert.Equal(newContributor.Id, updatedItem?.Id);
  }
}
