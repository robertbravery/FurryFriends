using FluentValidation;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Users.CreateUser;

public class CreateUserHandler : ICommandHandler<CreatePetWalkerCommand, Result<Guid>>
{
  private readonly IRepository<PetWalker> _petWalkerRepository;
  private readonly IValidator<CreatePetWalkerCommand> _validator;
  private readonly IValidator<Name> _nameValidator;
  private readonly IValidator<PhoneNumber> _phoneNumberValidator;

  public CreateUserHandler(IRepository<PetWalker> petWalkerRepository, IValidator<CreatePetWalkerCommand> commandValidator, IValidator<Name> nameValidator, IValidator<PhoneNumber> phoneNumberValidator)
  {
    _petWalkerRepository = petWalkerRepository;
    _validator = commandValidator;
    _nameValidator = nameValidator;
    _phoneNumberValidator = phoneNumberValidator;
  }

  public async Task<Result<Guid>> Handle(CreatePetWalkerCommand command, CancellationToken cancellationToken)
  {
    var validationResult = await _validator.ValidateAsync(command, cancellationToken);
    if (!validationResult.IsValid)
    {
      return Result<Guid>.Invalid(validationResult.Errors.Select(e => new ValidationError(e.ErrorMessage)).ToList());
    }

    var phoneNumberResult = await PhoneNumber.Create(command.CountryCode, command.Number, _phoneNumberValidator);
    if (!phoneNumberResult.IsSuccess)
    {
      return Result<Guid>.Invalid(phoneNumberResult.Errors.Select(e => new ValidationError(e)).ToList());
    }

    var addressResult = Address.Create(command.Street, command.City, command.State, command.Country, command.ZipCode);
    if (!addressResult.IsSuccess)
    {
      return Result<Guid>.Invalid(addressResult.Errors.Select(e => new ValidationError(e)).ToList());
    }

    var nameResult = Name.Create(command.FirstName, command.LastName, _nameValidator);
    if (!nameResult.IsSuccess)
    {
      return Result<Guid>.Invalid(nameResult.Errors.Select(e => new ValidationError(e)).ToList());
    }

    var emailResult = Email.Create(command.Email);
    if (!emailResult.IsSuccess)
    {
      return Result<Guid>.Invalid(emailResult.Errors.Select(e => new ValidationError(e)).ToList());
    }

    var genderResult = GenderType.Create(command.Gender);
    if (!genderResult.IsSuccess)
    {
      return Result<Guid>.Invalid(genderResult.Errors.Select(e => new ValidationError(e)).ToList());
    }

    var compensationResult = Compensation.Create(command.HourlyRate, command.Currency);
    if (!compensationResult.IsSuccess)
    {
      return Result<Guid>.Invalid(compensationResult.Errors.Select(e => new ValidationError(e)).ToList());
    }

    var user = PetWalker.Create(nameResult.Value, emailResult.Value, phoneNumberResult.Value, addressResult.Value);
    user.UpdateGender(genderResult.Value);
    user.UpdateBiography(command.Biography);
    user.UpdateDateOfBirth(command.DateOfBirth);
    user.UpdateIsActive(command.IsActive);
    user.UpdateIsVerified(command.IsVerified);
    user.UpdateYearsOfExperience(command.YearsOfExperience);
    user.UpdateHasInsurance(command.HasInsurance);
    user.UpdateHasFirstAidCertification(command.HasFirstAidCertification);
    user.UpdateDailyPetWalkLimit(command.DailyPetWalkLimit);
    user.UpdateCompensation(compensationResult.Value);

    var addedUser = await _petWalkerRepository.AddAsync(user, cancellationToken);

    return Result<Guid>.Success(addedUser.Id);
  }
}

