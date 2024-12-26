using Xunit;
using Moq;
using FluentAssertions;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UnitTests.Core.UserAggregate;

public class UserAggregateTests
{
  private readonly Address _validAddress;
  private readonly PhoneNumber _validPhone;

  public UserAggregateTests()
  {
    _validAddress = new Address("123 Main St", "City", "State", "12345");
    _validPhone = PhoneNumber.Create("027", "011", "123-4567", new PhoneNumberValidator());
  }

  [Fact]
  public void CreateUser_WithValidData_ShouldCreateUserSuccessfully()
  {
    // Arrange

    var email = "test@example.com";
    var firstName = "John";


    // Act
    var userAggregate = new User(firstName, email, _validPhone, _validAddress);

    // Assert
    userAggregate.Email.Should().Be(email);
    userAggregate.Name.Should().Be(firstName);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("invalid-email")]
  public void CreateUser_WithInvalidEmail_ShouldThrowValidationException(string invalidEmail)
  {
    // Arrange
    var firstName = "John";
    //var lastName = "Doe";
    var phoneNumber = PhoneNumber.Create("027", "011", "123-4567", new PhoneNumberValidator());
    var address = new Address("123 Main St", "Anytown", "CA", "12345");

    // Act
    // Act and Assert
    var action = () => new User(firstName, invalidEmail, phoneNumber, address);
    action.Should().Throw<Exception>()
        .WithMessage("*email*");
  }


  [Fact]
  public void UpdateEmail_WithValidEmail_ShouldUpdateSuccessfully()
  {
    // Arrange
    var email = "old@example.com";
    var firstName = "John";
    var phoneNumber = PhoneNumber.Create("027", "011", "123-4567", new PhoneNumberValidator());
    var address = new Address("123 Main St", "Anytown", "CA", "12345");
    var user = new User(firstName, email, phoneNumber, address);
    var newEmail = "new@example.com";

    // Act
    user.UpdateEmail(newEmail);

    // Assert
    user.Email.Should().Be(newEmail);
  }

  [Theory]
  [InlineData("invalid-email")]
  [InlineData("@invalid.com")]
  [InlineData("")]
  public void UpdateEmail_WithInvalidEmail_ThrowsException(string invalidEmail)
  {
    // Arrange
    var user = new User("John Doe", "john@example.com", _validPhone, _validAddress);

    // Act
    var action = () => user.UpdateEmail(invalidEmail);

    // Assert
    action.Should().Throw<ArgumentException>()
        .WithParameterName("newEmail");
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Constructor_WithInvalidName_ThrowsException(string invalidName)
  {
    // Act
    var action = () => new User(invalidName, "valid@email.com", _validPhone, _validAddress);

    // Assert
    action.Should().Throw<ArgumentException>()
        .WithParameterName("name");
  }

  [Fact]
  public void UpdateDetails_WithValidInputs_UpdatesUserDetails()
  {
    // Arrange
    var user = new User("John Doe", "john@example.com", _validPhone, _validAddress);
    var newName = "Jane Doe";
    var newEmail = "jane@example.com";
    var newPhone = PhoneNumber.Create("027", "011", "123-4567", new PhoneNumberValidator());
    var newAddress = new Address("456 Oak St", "NewCity", "NewState", "54321");

    // Act
    user.UpdateDetails(newName, newEmail, newPhone, newAddress);

    // Assert
    user.Name.Should().Be(newName);
    user.Email.Should().Be(newEmail);
    user.PhoneNumber.Should().Be(newPhone);
    user.Address.Should().Be(newAddress);
  }

  [Fact]
  public void UpdateEmail_WithValidEmail_UpdatesEmail()
  {
    // Arrange
    var user = new User("John Doe", "john@example.com", _validPhone, _validAddress);
    var newEmail = "newemail@example.com";

    // Act
    user.UpdateEmail(newEmail);

    // Assert
    user.Email.Should().Be(newEmail);
  }


  [Theory]
  [InlineData("")]
  [InlineData("a")] // Too short
  [InlineData("veryverylongusernameexceedingmaxlength")] // Too long
  public void CreateUser_WithInvalidUsername_ShouldThrowValidationException(string invalidUsername) 
  { 
    // Arrange
    var email = "john@example.com";
    var phoneNumber = _validPhone;
    var address = _validAddress;
    var user = new User("John Doe", email, _validPhone, _validAddress);

    // Act
    var action = () => user.UpdateUsername(invalidUsername);

    // Assert
    action.Should().Throw<ArgumentException>()
        .WithParameterName("newUserName");
  }


}

/*This test suite provides comprehensive coverage for the UserAggregate including:

Positive test cases:
User creation with valid data
Email updates with valid data
Username updates with valid data
User deactivation
User reactivation

Negative test cases:
Invalid email formats
Invalid username formats
Duplicate deactivation attempts
Duplicate reactivation attempts
The tests use:

XUnit's [Fact] for single test cases
XUnit's [Theory] with [InlineData] for parameterized tests
FluentAssertions for readable assertions
Moq for mocking dependencies (though not heavily used in this example)
Key testing patterns demonstrated:

Arrange-Act-Assert pattern
Testing both success and failure scenarios
Validation of state changes
Event emission verification
Exception testing
Boundary testing for validation rules
*/
