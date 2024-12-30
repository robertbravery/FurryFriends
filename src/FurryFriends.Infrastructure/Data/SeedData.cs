using System.Runtime.InteropServices;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.Infrastructure.Data;

public static class SeedData
{

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Contributors.AnyAsync()) return; // DB has been seeded

    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {

    await SeedData.PopulateContributorTestDataAsync(dbContext);
    await SeedData.PopulateUserTestDataAsync(dbContext);
    
  }

  private static async Task PopulateContributorTestDataAsync(AppDbContext dbContext)
  {
    Contributor Contributor1 = new(Name.Create("Snow", "Frog", "Snow Frog", new NameValidator()).Value);
    Contributor Contributor2 = new(Name.Create("Snow", "Dog", "Snow Dog", new NameValidator()).Value);
    var validator = new PhoneNumberValidator();
    await Contributor1.SetPhoneNumber("123-456-7890", validator);
    await Contributor2.SetPhoneNumber("987-654-3210", validator);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    await dbContext.SaveChangesAsync();

  }

  private static async Task PopulateUserTestDataAsync(AppDbContext dbContext)
  {
    var validator = new PhoneNumberValidator();
    var phoneNumber1 = await PhoneNumber.Create("027", "011", "123-4567", validator);
    var phoneNumber2 = await PhoneNumber.Create("027", "011", "789-1234", validator);
    var address1 = Address.Create("123 Test St", "Test City", "Test State", "12345");
    var address2 = Address.Create("456 Test St", "Test City", "Test State", "12345");

    var user1 = new User("Test User", "test@u.com", phoneNumber1, address1);
    var user2 = new User("Test User 2", "test2@u.com", phoneNumber2, address2);
    dbContext.Users.AddRange([user1, user2]);

    await dbContext.SaveChangesAsync();

  }
}
