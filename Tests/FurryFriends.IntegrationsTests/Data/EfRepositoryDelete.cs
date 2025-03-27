using FluentValidation;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;


namespace FurryFriends.IntegrationsTests.Data;

public class EfRepositoryDelete : BaseEfRepoTestFixture
{
  private readonly IValidator<Name> _nameValidator;

  public EfRepositoryDelete()
  {
    _nameValidator = new NameValidator();
  }

  [Fact]
  public async Task DeletesItemAfterAddingIt()
  {
    /// add a Contributor
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
    _dbContext.Entry(client).State = EntityState.Detached; ;

    // delete the item
    await repository.DeleteAsync(client, canellationToken);

    // verify it's no longer there
    var result = await repository.ListAsync(canellationToken);

    result.Should().NotContain(c => c.Name.FirstName == firstName);
  }
}
