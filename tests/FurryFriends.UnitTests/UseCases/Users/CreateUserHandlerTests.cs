using FluentValidation;
using FluentValidation.Results;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Users;
using FurryFriends.UseCases.Users.Create;
using Moq;

namespace FurryFriends.UseCases.Tests.Users;

public class CreateUserHandlerTests
{
  private readonly Mock<IRepository<User>> _userRepositoryMock;
  private readonly Mock<IValidator<CreateUserCommand>> _commandValidatorMock;
  private readonly Mock<IValidator<PhoneNumber>> _phoneNumberValidatorMock;
  private readonly CreateUserHandler _handler;

  public CreateUserHandlerTests()
  {
    _userRepositoryMock = new Mock<IRepository<User>>();
    _commandValidatorMock = new Mock<IValidator<CreateUserCommand>>();
    _phoneNumberValidatorMock = new Mock<IValidator<PhoneNumber>>();
    _handler = new CreateUserHandler(_userRepositoryMock.Object, _commandValidatorMock.Object, _phoneNumberValidatorMock.Object);
  }

  [Fact]
  public async Task Handle_ShouldReturnUserId_WhenCommandIsValid()
  {
    // Arrange
    var command = new CreateUserCommand(
        "John Doe",
        "john.doe@example.com",
        "1",
        "123",
        "4567890",
        "123 Main St",
        "Anytown",
        "CA",
        "12345"
    );

    _commandValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
        .ReturnsAsync(new ValidationResult());

    _phoneNumberValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PhoneNumber>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new ValidationResult());

    _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((User user, CancellationToken ct) => user);


    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Value.Should().NotBeEmpty();
  }

  [Fact]
  public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
  {
    // Arrange
    var command = new CreateUserCommand
    (
         "John Doe",
        "john.doe@example.com",
        "1",
        "123",
        "4567890",
        "123 Main St",
        "Anytown",
        "CA",
        "12345"
    );

    var validationResult = new ValidationResult(new[]
    {
            new ValidationFailure("Name", "Name cannot be empty")
        });

    _commandValidatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
        .ReturnsAsync(validationResult);

    // Act and Assert
    Func<Task> act =async  () => { await _handler.Handle(command, CancellationToken.None); };
    await act.Should().ThrowAsync<ValidationException>();
  }
}
