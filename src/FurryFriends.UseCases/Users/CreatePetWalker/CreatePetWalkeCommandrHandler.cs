using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Users.CreatePetWalker;

public class CreatePetWalkeCommandrHandler(IRepository<PetWalker> petWalkerRepository) : ICommandHandler<CreatePetWalkerCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreatePetWalkerCommand command, CancellationToken cancellationToken)
  {
    var phoneNumberCreationResult = await PhoneNumber.Create(command.CountryCode, command.Number);
    var addressCreationResult = Address.Create(command.Street, command.City, command.State, command.Country, command.ZipCode);
    var nameCreationResult = Name.Create(command.FirstName, command.LastName);
    var emailCreationResult = Email.Create(command.Email);
    var genderCreationResult = GenderType.Create(command.Gender);
    var compensationCreationResult = Compensation.Create(command.HourlyRate, command.Currency);

    var results = new IResult[]
    {
        nameCreationResult,
        emailCreationResult,
        phoneNumberCreationResult,
        addressCreationResult,
        genderCreationResult,
        compensationCreationResult
    };

    var errorsList = results.SelectMany(result => result.Errors);

    if (errorsList.Any())
    {

      return Result.Error(new ErrorList(errorsList));
    }

    var petWalker = PetWalker.Create(
        nameCreationResult.Value,
        emailCreationResult.Value,
        phoneNumberCreationResult.Value,
        addressCreationResult.Value);

    AddData(command, genderCreationResult, compensationCreationResult, petWalker);

    var addedPetWalker = await petWalkerRepository.AddAsync(petWalker, cancellationToken);

    return Result<Guid>.Success(addedPetWalker.Id);
  }

  private static void AddData(
    CreatePetWalkerCommand command,
    Result<GenderType> genderCreationResult,
    Result<Compensation> compensationCreationResult,
    PetWalker petWalker)
  {
    petWalker.UpdateGender(genderCreationResult.Value);
    petWalker.UpdateBiography(command.Biography);
    petWalker.UpdateDateOfBirth(command.DateOfBirth);
    petWalker.UpdateIsActive(command.IsActive);
    petWalker.UpdateIsVerified(command.IsVerified);
    petWalker.UpdateYearsOfExperience(command.YearsOfExperience);
    petWalker.UpdateHasInsurance(command.HasInsurance);
    petWalker.UpdateHasFirstAidCertification(command.HasFirstAidCertification);
    petWalker.UpdateDailyPetWalkLimit(command.DailyPetWalkLimit);
    petWalker.UpdateCompensation(compensationCreationResult.Value);
  }
}

