using MediatR;
using FluentValidation;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Users.Create;
using FurryFriends.Core.ValueObjects.Validators;
namespace FurryFriends.UseCases.Users;
public class CreateUserHandler(IRepository<User> userRepository, IValidator<CreateUserCommand> commandValidator, IValidator<PhoneNumber> phoneNumberValidator) 
: ICommandHandler<CreateUserCommand, Result<Guid>>
{
  private readonly IRepository<User> _userRepository = userRepository;
  private readonly IValidator<CreateUserCommand> _validator = commandValidator;
  private readonly IValidator<PhoneNumber> _phoneNumberValidator = phoneNumberValidator;

  public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    var validationResult = await _validator.ValidateAsync(command);
    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }
    var phoneNumberResult = await PhoneNumber.Create(command.CountryCode, command.AreaCode, command.Number, _phoneNumberValidator);
    if (!phoneNumberResult.IsSuccess)
    {
        return Result<Guid>.Invalid(new ValidationError(string.Join(", ", phoneNumberResult.Errors)));
    }
    var address = new Address(command.Street, command.City, command.State, command.ZipCode);
    var user = new User(command.Name, command.Email, phoneNumberResult.Value, address);
    await _userRepository.AddAsync(user);
    return user.Id;
  }
}