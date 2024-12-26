using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UnitTests.TestHelpers;

public static class UserHelpers
{
  public static List<User> GetTestUsers()
  {
    var phoneNumber = PhoneNumber.Create("027", "011", "1234567890",new PhoneNumberValidator());

    return new List<User>
      {
          new(
              "John Smith",
              "john.smith@example.com",
              phoneNumber,
              new Address("123 Main St", "Seattle", "WA", "98101")),

          new(
              "Jane Doe",
              "jane.doe@example.com",
              phoneNumber,
              new Address("456 Oak Ave", "Portland", "OR", "97201")),

          new(
              "Bob Wilson",
              "bob.wilson@example.com",
              phoneNumber,
              new Address("789 Pine St", "Seattle", "WA", "98102")),

          new(
              "Alice Brown",
              "alice.brown@example.com",
              phoneNumber,
              new Address("321 Cedar Rd", "Bellevue", "WA", "98004")),

          new(
              "Charlie Davis",
              "charlie.davis@example.com",
              phoneNumber,
              new Address("654 Elm St", "Seattle", "WA", "98103"))
      };
  }
}
