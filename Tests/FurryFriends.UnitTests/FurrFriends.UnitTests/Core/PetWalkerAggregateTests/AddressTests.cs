using Bogus;
using FluentValidation;
using FurryFriends.Core.ValueObjects;
using Moq;

namespace FurryFriends.UnitTests.Core.PetWalkerAggregate;

public class AddressTests
{
  private readonly Faker _faker;

  public AddressTests()
  {
    _faker = new Faker();
  }

  [Fact]
  public void Create_Address_WithValidData_ReturnsValidAddress()
  {
    // Arrange
    var street = _faker.Address.StreetName();
    var city = _faker.Address.City();
    var state = _faker.Address.State();
    var country = _faker.Address.Country();
    var zipCode = _faker.Address.ZipCode();

    // Act
    var result = Address.Create(street, city, state, country, zipCode).Value;

    // Assert
    result.Should().BeOfType<Address>();
    result.Street.Should().Be(street);
    result.City.Should().Be(city);
    result.StateProvinceRegion.Should().Be(state);
    result.Country.Should().Be(country);
    result.ZipCode.Should().Be(zipCode);
  }

  [Fact]
  public void Create_Address_WithInvalidData_ReturnsError()
  {
    // Arrange
    var street = string.Empty;
    var city = string.Empty;
    var state = string.Empty;
    var country = string.Empty;
    var zipCode = string.Empty;

    // Act
    var result = Address.Create(street, city, state, country, zipCode);

    // Assert
    //result.Should().BeOfType<Error>();
    result.ValidationErrors.Should().HaveCount(6);
  }

  [Fact]
  public void Validate_Address_WithValidData_ReturnsValidResult()
  {
    // Arrange
    var validator = new Mock<IValidator<Address>>();


    // Act
    var addressResult = Address.Create(_faker.Address.StreetName(), _faker.Address.City(), _faker.Address.State(), _faker.Address.Country(), _faker.Address.ZipCode());

    // Assert
    addressResult.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void Equals_Address_WithSameData_ReturnsTrue()
  {
    // Arrange
    var address1 = Address.Create(_faker.Address.StreetName(), _faker.Address.City(), _faker.Address.State(), _faker.Address.Country(), _faker.Address.ZipCode()).Value;
    var address2 = Address.Create(address1.Street, address1.City, address1.StateProvinceRegion, address1.Country, address1.ZipCode);

    // Act
    var result = address1.Equals(address2);

    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public void Equals_Address_WithDifferentData_ReturnsFalse()
  {
    // Arrange
    var address1 = Address.Create(_faker.Address.StreetName(), _faker.Address.City(), _faker.Address.State(), _faker.Address.Country(), _faker.Address.ZipCode());
    var address2 = Address.Create(_faker.Address.StreetName(), _faker.Address.City(), _faker.Address.State(), _faker.Address.Country(), _faker.Address.ZipCode());

    // Act
    var result = address1.Equals(address2);

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public void GetHashCode_Address_WithSameData_ReturnsSameHashCode()
  {
    // Arrange
    var streetName = _faker.Address.StreetName();
    var city = _faker.Address.City();
    var state = _faker.Address.State();
    var country = _faker.Address.Country();
    var zipCode = _faker.Address.ZipCode();

    var address1 = Address.Create(streetName, city, state, country, zipCode).Value;
    var address2 = Address.Create(streetName, city, state, country, zipCode).Value;

    // Act
    var hash1 = address1.GetHashCode();
    var hash2 = address2.GetHashCode();

    // Assert
    hash1.Should().Be(hash2);
  }
}
