using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.UnitTests.TestHelpers;

public static class UserHelpers
{
  public static async Task<List<User>> GetTestUsers()
  {
    var phoneNumberResult = await PhoneNumber.Create("027", "011-123-4567",new PhoneNumberValidator());
    var phoneNumber = phoneNumberResult.Value;
    var validator = new NameValidator();
    return new List<User>
      {
          User.Create(
              Name.Create("John", "Smith", validator),
              "john.smith@example.com",
              phoneNumber,
              Address.Create("123 Main St", "Seattle", "WA", "US", "98101")),

          User.Create(
              Name.Create("Jane", "Doe", validator), 
              "jane.doe@example.com",
              phoneNumber,
              Address.Create("456 Oak Ave", "Portland", "OR", "Us", "97201")),

          User.Create(
              Name.Create("Bob", "Wilson", validator),
              "bob.wilson@example.com",
              phoneNumber,
              Address.Create("789 Pine St", "Seattle", "WA", "US", "98102")),

          User.Create(
              Name.Create("Alice", "Brown", validator),
              "alice.brown@example.com",
              phoneNumber,
              Address.Create("321 Cedar Rd", "Bellevue", "WA", "US", "98004")),

          User.Create(
              Name.Create("Charlie", "Davis", validator),
              "charlie.davis@example.com",
              phoneNumber,
              Address.Create("654 Elm St", "Seattle", "WA", "US", "98103"))
      };
  }
}
