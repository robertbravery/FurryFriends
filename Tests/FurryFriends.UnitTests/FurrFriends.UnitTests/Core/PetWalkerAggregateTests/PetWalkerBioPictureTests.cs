﻿using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using Moq;

namespace FurryFriends.UnitTests.Core.PetWalkerAggregateTests;

public class PetWalkerBioPictureTests
{
  private readonly Mock<IRepository<PetWalker>> _userRepositoryMock;
  private readonly PetWalker _testUser;

  public PetWalkerBioPictureTests()
  {
    // Arrange
    var nameValidator = new NameValidator();
    var phoneValidator = new PhoneNumberValidator();
    var name = Name.Create("John", "Doe");
    var phone = PhoneNumber.Create("021", "123-456-7890").GetAwaiter().GetResult();
    var address = Address.Create("123 Main St", "City", "State", "US", "12345");
    var email = Email.Create("john@example.com");

    _userRepositoryMock = new Mock<IRepository<PetWalker>>();
    _testUser = PetWalker.Create(name, email, phone, address);
  }

  [Fact]
  public async Task AddBioPicture_WithValidPhoto_SetsBioPictureAsync()
  {
    // Arrange
    var name = Name.Create("John", "Doe");
    var address = Address.Create("123 Main St", "City", "State", "US", "12345");
    var phone = await PhoneNumber.Create("021", "123-456-7890");
    var email = Email.Create("john@example.com");
    var user = PetWalker.Create(name, email, phone, address);
    var photo = new Photo("http://localhost/test.jpg", "Some Description");

    // Act
    user.AddPhoto(photo);

    // Assert
    user.Photos.Should().Contain(photo);
  }

  [Fact]
  public async Task AddBioPicture_WithNullPhoto_ThrowsArgumentNullExceptionAsync()
  {
    // Arrange
    var name = Name.Create("John", "Doe");
    var address = Address.Create("123 Main St", "City", "State", "US", "12345");
    var phone = await PhoneNumber.Create("021", "123-456-7890");
    var email = Email.Create("john@example.com");
    var user = PetWalker.Create(name, email, phone, address);
    Photo nullPhoto = null!;

    // Act & Assert
    var action = () => user.AddPhoto(nullPhoto);
    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public async Task AddBioPicture_WithValidPhoto_CanUpdateExistingBioPictureAsync()
  {
    // Arrange
    var name = Name.Create("John", "Doe");
    var address = Address.Create("123 Main St", "City", "State", "US", "12345");
    var phone = await PhoneNumber.Create("021", "123-456-7890");
    var email = Email.Create("john@example.com");
    var user = PetWalker.Create(name, email, phone, address);
    var initialPhoto = new Photo("http://localhost/initial.jpg", "Some Description");
    var newPhoto = new Photo("http://localhost/new.jpg", "Some Description");

    user.AddPhoto(initialPhoto);

    // Act
    user.AddPhoto(newPhoto);

    // Assert
    user.Photos.Should().NotBeNull();
    user.Photos.Should().NotBeEmpty();
    user.Photos.Should().Contain(newPhoto);
  }
}
