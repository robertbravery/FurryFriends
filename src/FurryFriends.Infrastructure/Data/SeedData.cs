using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;

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
    Contributor1.SetPhoneNumber("123-456-7890", validator);
    Contributor2.SetPhoneNumber("987-654-3210", validator);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    await dbContext.SaveChangesAsync();

  }

  private static async Task PopulateUserTestDataAsync(AppDbContext dbContext)
  {
    var validator = new PhoneNumberValidator();
    var phoneNumberValidator1 = PhoneNumber.Create("027", "011", "123-4567", validator);
    var phoneNumberValidator = PhoneNumber.Create("027", "011", "789-1234", validator);
    await dbContext.SaveChangesAsync();

  }
}
