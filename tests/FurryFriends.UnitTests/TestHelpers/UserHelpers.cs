using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.UnitTests.TestHelpers;

public static class UserHelpers
{
  public static async Task<List<User>> GetTestUsers()
  {
    var phoneNumberResult = await PhoneNumber.Create("027", "011", "1234567890",new PhoneNumberValidator());
    var phoneNumber = phoneNumberResult.Value;
    var validator = new NameValidator();
    return new List<User>
      {
          new(
              Name.Create("John", "Smith", validator),
              "john.smith@example.com",
              phoneNumber,
              new Address("123 Main St", "Seattle", "WA", "98101")),

          new(
              Name.Create("Jane", "Doe", validator), 
              "jane.doe@example.com",
              phoneNumber,
              new Address("456 Oak Ave", "Portland", "OR", "97201")),

          new(
              Name.Create("Bob", "Wilson", validator),
              "bob.wilson@example.com",
              phoneNumber,
              new Address("789 Pine St", "Seattle", "WA", "98102")),

          new(
              Name.Create("Alice", "Brown", validator),
              "alice.brown@example.com",
              phoneNumber,
              new Address("321 Cedar Rd", "Bellevue", "WA", "98004")),

          new(
              Name.Create("Charlie", "Davis", validator),
              "charlie.davis@example.com",
              phoneNumber,
              new Address("654 Elm St", "Seattle", "WA", "98103"))
      };
  }
}
