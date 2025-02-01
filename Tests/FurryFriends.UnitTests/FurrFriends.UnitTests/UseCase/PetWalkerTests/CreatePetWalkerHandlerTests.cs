using Bogus;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UnitTests.TestHelpers;
using FurryFriends.UseCases.PetWalkers.CreatePetWalker;
using FurryFriends.UseCases.Services;
using Moq;

namespace FurryFriends.UnitTests.UseCase.PetWalkerTests;

public class CreatePetWalkerHandlerTests
{
  private readonly Mock<IPetWalkerService> _petWalkerServiceMock;
  private readonly CreatePetWalkeCommandrHandler _handler;

  public CreatePetWalkerHandlerTests()
  {
    _petWalkerServiceMock = new Mock<IPetWalkerService>();
    _handler = new CreatePetWalkeCommandrHandler(_petWalkerServiceMock.Object);
  }

  [Fact]
  public async Task Handle_ShouldReturnUserId_WhenCommandIsValid()
  {
    // Arrange
    var f = new Faker();
    var command = new CreatePetWalkerCommand(
        f.Name.FirstName(),
        f.Name.LastName(),
        f.Internet.Email(),
        f.Phone.PhoneNumber("0##"),
        f.Phone.PhoneNumber("###-###-####"),
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.Country(),
        f.Address.ZipCode("####"),
        GenderType.GenderCategory.Male, // Assuming a default value for Gender
        null, // Assuming Biography can be null
        DateTime.Now, // Assuming a default value for DateOfBirth
        110m, // Assuming a default value for HourlyRate
        "USD", // Assuming a default value for Currency
        true, // Assuming a default value for IsActive
        false, // Assuming a default value for IsVerified
        0, // Assuming a default value for YearsOfExperience
        false, // Assuming a default value for HasInsurance
        false, // Assuming a default value for HasFirstAidCertification
        0 // Assuming a default value for DailyPetWalkLimit
    );
    var user = (await PetWalkerHelpers.GetTestUsers()).First();

    _petWalkerServiceMock.Setup(r => r.CreatePetWalkerAsync(It.IsAny<CreatePetWalkerDto>()))
        .ReturnsAsync(user);


    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Value.Should().NotBeEmpty();
  }

  [Fact]
  public async Task Handle_ShouldReturnErrorWhenNameIsEmpty()
  {
    // Arrange
    var f = new Faker();
    var command = new CreatePetWalkerCommand(
        string.Empty,
        f.Name.LastName(),
        f.Internet.Email(),
        f.Phone.PhoneNumber("0##"),
        f.Phone.PhoneNumber("###-###-####"),
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.Country(), f.Address.ZipCode("####"),
         GenderType.GenderCategory.Male, // Assuming a default value for Gender
        null, // Assuming Biography can be null
        DateTime.Now, // Assuming a default value for DateOfBirth
        10m, // Assuming a default value for HourlyRate
        "USD", // Assuming a default value for Currency
        true, // Assuming a default value for IsActive
        false, // Assuming a default value for IsVerified
        0, // Assuming a default value for YearsOfExperience
        false, // Assuming a default value for HasInsurance
        false, // Assuming a default value for HasFirstAidCertification
        0 // Assuming a default value for DailyPetWalkLimit
    );

    //Act
    var result = await _handler.Handle(command, CancellationToken.None);

    //Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().NotBeEmpty();
    result.ValidationErrors.First().ErrorMessage.Should().Contain("First name cannot be null or whitespace");
  }

  [Fact]
  public async Task Handle_ShouldReturnErrorWhenCountryCodeIsEmpty()
  {
    // Arrange
    var f = new Faker();
    var command = new CreatePetWalkerCommand(
        f.Name.FirstName(),
        f.Name.LastName(),
        f.Internet.Email(),
        string.Empty,
        f.Phone.PhoneNumber(),
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.Country(),
        f.Address.ZipCode("####"),
        GenderType.GenderCategory.Male, // Assuming a default value for Gender
        null, // Assuming Biography can be null
        DateTime.Now, // Assuming a default value for DateOfBirth
        10m, // Assuming a default value for HourlyRate
        "USD", // Assuming a default value for Currency
        true, // Assuming a default value for IsActive
        false, // Assuming a default value for IsVerified
        0, // Assuming a default value for YearsOfExperience
        false, // Assuming a default value for HasInsurance
        false, // Assuming a default value for HasFirstAidCertification
        0 // Assuming a default value for DailyPetWalkLimit
    );

    //Act
    var result = await _handler.Handle(command, CancellationToken.None);

    //Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().NotBeEmpty();
    result.ValidationErrors.First().ErrorMessage.Should().Contain("Country code is required");
  }

  [Fact]
  public async Task Handle_ShouldReturnErrorWhenPhoneNumberIsEmpty()
  {
    // Arrange
    var f = new Faker();
    var command = new CreatePetWalkerCommand(
        f.Name.FirstName(),
        f.Name.LastName(),
        f.Internet.Email(),
        f.Phone.PhoneNumber("0##"),
        string.Empty,
        f.Address.StreetAddress(),
        f.Address.City(),
        f.Address.State(),
        f.Address.Country(),
        f.Address.ZipCode("####"),
         GenderType.GenderCategory.Male, // Assuming a default value for Gender
        null, // Assuming Biography can be null
        DateTime.Now, // Assuming a default value for DateOfBirth
        10m, // Assuming a default value for HourlyRate
        "USD", // Assuming a default value for Currency
        true, // Assuming a default value for IsActive
        false, // Assuming a default value for IsVerified
        0, // Assuming a default value for YearsOfExperience
        false, // Assuming a default value for HasInsurance
        false, // Assuming a default value for HasFirstAidCertification
        0 // Assuming a default value for DailyPetWalkLimit
    );

    //Act
    var result = await _handler.Handle(command, CancellationToken.None);

    //Assert
    result.IsSuccess.Should().BeFalse();
    result.ValidationErrors.Should().NotBeEmpty();
    result.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("Valid Phonenumber is required"));
  }
}
