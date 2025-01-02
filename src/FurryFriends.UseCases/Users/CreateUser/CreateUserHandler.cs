using FluentValidation;
using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Users.CreateUser;

public class CreateUserHandler(IRepository<User> userRepository, IValidator<CreateUserCommand> commandValidator, IValidator<Name> nameValidator, IValidator<PhoneNumber> phoneNumberValidator)
: ICommandHandler<CreateUserCommand, Result<Guid>>
{
  private readonly IRepository<User> _userRepository = userRepository;
  private readonly IValidator<CreateUserCommand> _validator = commandValidator;
  private readonly IValidator<Name> _nameValidator = nameValidator;
  private readonly IValidator<PhoneNumber> _phoneNumberValidator = phoneNumberValidator;

  public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    var validationResult = await _validator.ValidateAsync(command);
    if (!validationResult.IsValid)
    {
      return Result<Guid>.Invalid(new ValidationError(string.Join(", ", validationResult.Errors)));
    }
    var phoneNumberResult = await PhoneNumber.Create(command.CountryCode, command.Number, _phoneNumberValidator);
    if (!phoneNumberResult.IsSuccess)
    {
      return Result<Guid>.Invalid(new ValidationError(string.Join(", ", phoneNumberResult.Errors)));
    }
    var address = Address.Create(command.Street, command.City, command.State, command.State, command.ZipCode);
    var name = Name.Create(command.FirstName, command.LastName, _nameValidator);
    var user = User.Create(name, command.Email, phoneNumberResult.Value, address);
    await _userRepository.AddAsync(user);
    return user.Id;
  }
}
