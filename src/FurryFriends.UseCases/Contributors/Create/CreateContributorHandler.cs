using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Contributors.Create;

public class CreateContributorHandler(IRepository<Contributor> _repository, IValidator<PhoneNumber> validator)
  : ICommandHandler<CreateContributorCommand, Result<int>>
{
  public async Task<Result<int>> Handle(CreateContributorCommand request,
    CancellationToken cancellationToken)
  {
    var nameParts = request.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    var firstName = string.IsNullOrEmpty(nameParts[0]) ? "Unknown" : nameParts[0];
    var lastName = string.IsNullOrEmpty(nameParts[1]) ? "Unknown" : nameParts[1];
    var fullName = string.Join(" ", nameParts);
    var name = Name.Create(firstName, lastName, new NameValidator());
    var newContributor = new Contributor(name);
    if (!string.IsNullOrEmpty(request.PhoneNumber))
    {
      await newContributor.SetPhoneNumber(request.PhoneNumber, validator);
    }
    var createdItem = await _repository.AddAsync(newContributor, cancellationToken);

    return createdItem.Id;
  }
}
