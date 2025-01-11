using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.PetWalkerAggregate;
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
    Contributor Contributor1 = new(Name.Create("Snow", "Frog").Value);
    Contributor Contributor2 = new(Name.Create("Snow", "Dog").Value);
    var validator = new PhoneNumberValidator();
    await Contributor1.SetPhoneNumber("123-456-7890", validator);
    await Contributor2.SetPhoneNumber("987-654-3210", validator);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    await dbContext.SaveChangesAsync();

  }

  private static async Task PopulateUserTestDataAsync(AppDbContext dbContext)
  {
    var validator = new PhoneNumberValidator();
    var phoneNumber1 = await PhoneNumber.Create("027", "011-123-4567");
    var phoneNumber2 = await PhoneNumber.Create("027", "011-123-4567");
    var address1 = Address.Create("123 Test St", "Test City", "Test State", "US", "12345");
    var address2 = Address.Create("456 Test St", "Test City", "Test State", "US", "12345");
    var name1 = Name.Create("Snow", "Frog").Value;
    var name2 = Name.Create("Snow", "Dog").Value;
    var email1 = Email.Create("test@u.com");
    var email2 = Email.Create("test@u.com");
    var user1 = PetWalker.Create(name1, email1, phoneNumber1, address1);
    var user2 = PetWalker.Create(name2, email2, phoneNumber2, address2);
    dbContext.PetWalkers.AddRange(new List<PetWalker> { user1, user2 });

    await dbContext.SaveChangesAsync();

  }
}
