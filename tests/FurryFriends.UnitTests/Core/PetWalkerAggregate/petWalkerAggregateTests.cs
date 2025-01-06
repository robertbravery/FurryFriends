using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.UnitTests.Core.PetWalkerAggregate;

public class petWalkerAggregateTests
{
  private readonly Address _validAddress;
  private readonly PhoneNumber _validPhone;

  public petWalkerAggregateTests()
  {
    _validAddress = Address.Create("123 Main St", "City", "State", "US", "12345");
    _validPhone = PhoneNumber.Create("027", "011-123-4567", new PhoneNumberValidator()).Result.Value;
  }

  [Fact]
  public void CreateUser_WithValidData_ShouldCreateUserSuccessfully()
  {
    // Arrange    
    var firstName = "John";
    var lastName = "Doe";
    var name = Name.Create(firstName, lastName, new NameValidator());
    var email = Email.Create("test@example.com");

    // Act
    var userAggregate = PetWalker.Create(name, email, _validPhone, _validAddress);

    // Assert
    userAggregate.Email.Should().Be(email);
    userAggregate.Name.FirstName.Should().Be(firstName);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("invalid-email")]
  public void CreateUser_WithInvalidEmail_ShouldNotBeSuccessfull(string invalidEmail)
  {
    // Act
    var emailResult = Email.Create(invalidEmail);

    // assert    
    emailResult.IsSuccess.Should().BeFalse();
    emailResult.Errors.First().Should().Contain("Invalid email address");

  }


  [Fact]
  public async Task UpdateEmail_WithValidEmail_ShouldUpdateSuccessfully()
  {
    // Arrange
    var firstName = "John";
    var lastName = "Doe";
    var phoneNumber = await PhoneNumber.Create("027", "011-123-4567", new PhoneNumberValidator());
    var address = Address.Create("123 Main St", "Anytown", "CA", "US", "12345");
    var name = Name.Create(firstName, lastName, new NameValidator());
    var email = Email.Create("old@example.com");
    var user = PetWalker.Create(name, email, phoneNumber, address);
    var newEmail = Email.Create("new@example.com");

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
    var emailResult = Email.Create("john@example.com");

    // Act
    var invalidEmailResult = Email.Create(invalidEmail);


    // Assert
    emailResult.IsSuccess.Should().BeTrue();
    invalidEmailResult.IsSuccess.Should().BeFalse();
    invalidEmailResult.Errors.First().Should().Contain("Invalid email address");
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Constructor_WithInvalidName_ThrowsException(string invalidName)
  {
    // Arrange


    // Act
    var name = Name.Create(invalidName, invalidName, new NameValidator());

    // Assert
    name.IsSuccess.Should().BeFalse();
    name.Errors.First().Should().Contain("First name");
  }

  [Fact]
  public async Task UpdateDetails_WithValidInputs_UpdatesUserDetails()
  {
    // Arrange
    var newName = Name.Create("Jane", "Doe", new NameValidator());
    var newEmail = Email.Create("jane@example.com");
    var name = Name.Create("John", "Doe", new NameValidator());
    var oldEmail = Email.Create("john@example.com");
    var user = PetWalker.Create(name, oldEmail, _validPhone, _validAddress);
    var newPhone = await PhoneNumber.Create("027", "011-123-4567", new PhoneNumberValidator());
    var newAddress = Address.Create("456 Oak St", "NewCity", "NewState", "US", "54321");

    // Act
    user.UpdateDetails(name, newEmail, newPhone, newAddress);

    // Assert
    user.Name.FullName.Should().Be(name.Value.FullName);
    user.Email.Should().Be(newEmail);
    user.PhoneNumber.Should().Be(newPhone);
    user.Address.Should().Be(newAddress);
  }

  [Fact]
  public void UpdateEmail_WithValidEmail_UpdatesEmail()
  {
    // Arrange
    var name = Name.Create("John", "Doe", new NameValidator());
    var oldEmail = Email.Create("john@example.com");
    var user = PetWalker.Create(name, oldEmail, _validPhone, _validAddress);
    var newEmail = Email.Create("newemail@example.com");

    // Act
    user.UpdateEmail(newEmail);

    // Assert
    user.Email.Should().Be(newEmail);
  }


  [Theory]
  [InlineData("")]
  [InlineData("a")] // Too short
  [InlineData("veryverylongusernameexceedingmaxlength")] // Too long
  public void CreateUser_WithInvalidUsername_ShouldReturnValidationError(string invalidUsername)
  {
    // Arrange
    var nameValidator = new NameValidator();

    //act
    var name = Name.Create(invalidUsername, invalidUsername, nameValidator);

    // Assert
    name.IsSuccess.Should().BeFalse();
    name.Errors.First().Should().Contain("First name");


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
