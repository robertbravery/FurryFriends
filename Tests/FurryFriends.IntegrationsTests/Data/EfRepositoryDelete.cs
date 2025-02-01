using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
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
    // add a Contributor
    var repository = GetRepository();
    var firstName = "Joe";
    var lastName = "Soap";
    var initialName = Name.Create(firstName, lastName).Value;
    var Contributor = new Contributor(initialName);
    var canellationToken = new CancellationToken();

    await repository.AddAsync(Contributor, canellationToken);

    // delete the item
    await repository.DeleteAsync(Contributor, canellationToken);

    // verify it's no longer there
    var result = await repository.ListAsync(canellationToken);

    result.Should().NotContain(c => c.Name.FullName == initialName.FullName);
  }
}
