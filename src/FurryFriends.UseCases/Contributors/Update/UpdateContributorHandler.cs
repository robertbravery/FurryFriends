using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCase.Contributors.Update;

public class UpdateContributorHandler(
    IRepository<Contributor> _repository)
    : ICommandHandler<UpdateContributorCommand, Result<ContributorDTO>>
{
  public async Task<Result<ContributorDTO>> Handle(UpdateContributorCommand request, CancellationToken cancellationToken)
  {
    var existingContributor = await _repository.GetByIdAsync(request.ContributorId, cancellationToken);
    if (existingContributor == null)
    {
      return Result.NotFound();
    }
    var nameParts = request.NewName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    var firstName = string.IsNullOrEmpty(nameParts[0]) ? "Unknown" : nameParts[0];
    var lastName = string.IsNullOrEmpty(nameParts[1]) ? "Unknown" : nameParts[1];
    var fullName = string.Join(" ", nameParts);
    var name = Name.Create(firstName, lastName).Value;
    existingContributor.UpdateName(name);

    await _repository.UpdateAsync(existingContributor, cancellationToken);

    return new ContributorDTO(existingContributor.Id,
      existingContributor.Name.FullName, existingContributor.PhoneNumber?.Number ?? "");
  }
}
