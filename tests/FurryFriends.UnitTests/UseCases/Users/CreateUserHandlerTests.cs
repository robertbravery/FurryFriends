﻿using Bogus;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users;
using FurryFriends.UseCases.Users.Create;
using Moq;

namespace FurryFriends.UseCases.Tests.Users;

public class CreateUserHandlerTests
{
  private readonly Mock<IRepository<User>> _userRepositoryMock;
  private readonly CreateUserHandler _handler;

  public CreateUserHandlerTests()
  {
    _userRepositoryMock = new Mock<IRepository<User>>();
    _handler = new CreateUserHandler(_userRepositoryMock.Object, new CreateUserCommandValidator(), new NameValidator(), new PhoneNumberValidator());
  }

  [Fact]
  public async Task Handle_ShouldReturnUserId_WhenCommandIsValid()
  {
    // Arrange
    var f = new Faker();
    var command = new CreateUserCommand(
        f.Name.FirstName(), 
        f.Name.LastName(),
        f.Internet.Email(),
        f.Phone.PhoneNumber("0##"),
        f.Phone.PhoneNumber("###-###-####"),
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.ZipCode("####")
    );

    _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((User user, CancellationToken ct) => user);


    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Value.Should().NotBeEmpty();
  }

  [Fact]
  public async Task Handle_ShouldReturnErrorWhenNameIsEmpty()
  {
    // Arrange
    //var expectedErrorMessage = "Name cannot be empty";
    var f = new Faker();
    var command = new CreateUserCommand(
        string.Empty,
        f.Name.LastName(), 
        f.Internet.Email(),
        f.Phone.PhoneNumber("0##"),
        f.Phone.PhoneNumber("###-###-####"),
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.ZipCode("####")
    );

    //Act
    var result = await _handler.Handle(command, CancellationToken.None);

    //Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().NotBeEmpty();
    result.ValidationErrors.First().ErrorMessage.Should().Contain("Name cannot be empty");
  }

  [Fact]
  public async Task Handle_ShouldReturnErrorWhenCountryCodeIsEmpty()
  {
    // Arrange
    //var expectedErrorMessage = "Name cannot be empty";
    var f = new Faker();
    var command = new CreateUserCommand(
        f.Name.FirstName(),
        f.Name.LastName(), 
        f.Internet.Email(),
        string.Empty,
        f.Phone.PhoneNumber(),
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.ZipCode("####")
    );

    //Act
    var result = await _handler.Handle(command, CancellationToken.None);

    //Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().NotBeEmpty();
    result.ValidationErrors.First().ErrorMessage.Should().Contain("Country code cannot be empty");
  }

  

  [Fact]
  public async Task Handle_ShouldReturnErrorWhenPhoneNumberIsEmpty()
  {
    // Arrange
    //var expectedErrorMessage = "Name cannot be empty";
    var f = new Faker();
    var command = new CreateUserCommand(
        f.Name.FirstName(),
        f.Name.LastName(), 
        f.Internet.Email(),
        f.Phone.PhoneNumber("0##"),
        string.Empty,
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.ZipCode("####")
    );

    //Act
    var result = await _handler.Handle(command, CancellationToken.None);

    //Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().NotBeEmpty();
    result.ValidationErrors.First().ErrorMessage.Should().Contain("Phone number cannot be empty");
  }

 
}
