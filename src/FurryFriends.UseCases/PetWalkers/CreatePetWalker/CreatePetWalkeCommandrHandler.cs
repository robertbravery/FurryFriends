using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCase.Users.CreatePetWalker;
using FurryFriends.UseCases.Services;
using FurryFriends.UseCases.Users.CreatePetWalker;

namespace FurryFriends.UseCases.PetWalkers.CreatePetWalker;

public class CreatePetWalkeCommandrHandler(IPetWalkerService petWalkerService) : ICommandHandler<CreatePetWalkerCommand, Result<Guid>>
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

    var errorsList = results.SelectMany(result => result.ValidationErrors);

    if (errorsList.Any())
    {

      return Result.Invalid(errorsList);
    }

    var createPetWalkerDto = new CreatePetWalkerDto(
       nameCreationResult.Value,
       emailCreationResult.Value,
       phoneNumberCreationResult.Value,
       addressCreationResult.Value,
       genderCreationResult.Value,
       command.Biography ?? string.Empty, // provide a default value if null
       command.DateOfBirth,
       command.IsActive,
       command.IsVerified,
       command.YearsOfExperience,
       command.HasInsurance,
       command.HasFirstAidCertification,
       command.DailyPetWalkLimit,
       compensationCreationResult.Value);

    var result = await petWalkerService.CreatePetWalkerAsync(createPetWalkerDto);


    return result.IsSuccess ?
        Result<Guid>.Success(result.Value.Id) :
         Result<Guid>.Error(string.Join(", ", result.Errors));
  }

}

