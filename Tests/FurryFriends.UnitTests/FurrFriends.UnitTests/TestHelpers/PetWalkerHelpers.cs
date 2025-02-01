using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.UnitTests.TestHelpers;

public static class PetWalkerHelpers
{
  public static async Task<List<PetWalker>> GetTestUsers()
  {
    var phoneNumberResult = await PhoneNumber.Create("027", "011-123-4567");
    var phoneNumber = phoneNumberResult.Value;
    var validator = new NameValidator();
    return new List<PetWalker>
      {
          PetWalker.Create(
              Name.Create("John", "Smith"),
              Email.Create("john.smith@example.com"),
              phoneNumber,
              Address.Create("123 Main St", "Seattle", "WA", "US", "98101")),

          PetWalker.Create(
              Name.Create("Jane", "Doe"),
              Email.Create("jane.doe@example.com"),
              phoneNumber,
              Address.Create("456 Oak Ave", "Portland", "OR", "Us", "97201")),

          PetWalker.Create(
              Name.Create("Bob", "Wilson"),
              Email.Create("bob.wilson@example.com"),
              phoneNumber,
              Address.Create("789 Pine St", "Seattle", "WA", "US", "98102")),

          PetWalker.Create(
              Name.Create("Alice", "Brown"),
              Email.Create("alice.brown@example.com"),
              phoneNumber,
              Address.Create("321 Cedar Rd", "Bellevue", "WA", "US", "98004")),

          PetWalker.Create(
              Name.Create("Charlie", "Davis"),
              Email.Create("charlie.davis@example.com"),
              phoneNumber,
              Address.Create("654 Elm St", "Seattle", "WA", "US", "98103"))
      };
  }
}
