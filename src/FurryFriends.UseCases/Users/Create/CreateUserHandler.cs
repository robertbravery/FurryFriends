using MediatR;
using FluentValidation;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Users.Create;
namespace FurryFriends.UseCases.Users;
public class CreateUserHandler(IRepository<User> userRepository, IValidator<CreateUserCommand> commandValidator, IValidator<PhoneNumber> phoneNumberValidator) 
: ICommandHandler<CreateUserCommand, Result<Guid>>
{
  private readonly IRepository<User> _userRepository = userRepository;
  private readonly IValidator<CreateUserCommand> _validator = commandValidator;
  private readonly IValidator<PhoneNumber> _phoneNumberValidator = phoneNumberValidator;

  public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
  {
    var validationResult = _validator.Validate(request);
    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }
    var phoneNumber = PhoneNumber.Create(request.CountryCode, request.AreaCode, request.Number, _phoneNumberValidator);
    var address = new Address(request.Street, request.City, request.State, request.ZipCode);
    var user = new User(request.Name, request.Email, phoneNumber, address);
    await _userRepository.AddAsync(user);
    return user.Id;
  }
}