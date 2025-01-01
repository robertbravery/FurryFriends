using FurryFriends.Core.ContributorAggregate;
using FluentValidation;
using FurryFriends.Core.ValueObjects;
using FluentAssertions;


namespace FurryFriends.IntegrationTests.Data;

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
    var initialName = Name.Create(firstName, lastName, _nameValidator).Value;
    var Contributor = new Contributor(initialName);
    await repository.AddAsync(Contributor);

    // delete the item
    await repository.DeleteAsync(Contributor);

    // verify it's no longer there
    var result = await repository.ListAsync();

    result.Should().NotContain(c=> c.Name.FullName == initialName.FullName);
  }
}
