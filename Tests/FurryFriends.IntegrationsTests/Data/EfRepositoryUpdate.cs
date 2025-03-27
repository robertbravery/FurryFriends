using FurryFriends.Core.ClientAggregate;
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
    var email = Email.Create("X9d0z@example.com");
    var phone = await PhoneNumber.Create("27", "123456789");
    var address = Address.Create("123 Main St", "Anytown", "CA", "Any country", "12345");
    var client = Client.Create(initialName, email, phone, address);
    var canellationToken = new CancellationToken();


    await repository.AddAsync(client, canellationToken);

    // detach the item so we get a different instance
    _dbContext.Entry(client).State = EntityState.Detached;

    // fetch the item and update its title
    var newClient = (await repository.ListAsync(canellationToken))
        .FirstOrDefault(Contributor => Contributor.Name == initialName);
    if (newClient == null)
    {
      Assert.NotNull(newClient);
      return;
    }

    newClient.Should().NotBeSameAs(client);

    var newemail = Email.Create("X9d0z@example.com");
    newClient.UpdateEmail(newemail);

    // Update the item
    await repository.UpdateAsync(newClient, canellationToken);

    // Fetch the updated item
    var updatedItem = (await repository.ListAsync(canellationToken))
        .FirstOrDefault(client => client.Email == newemail);

    updatedItem.Should().NotBeNull();
    updatedItem?.Email.Should().NotBe(client.Email);
    updatedItem?.Name.Should().Be(newemail);
    //updatedItem?.Status.Should().Be(client.Status);
  }
}
