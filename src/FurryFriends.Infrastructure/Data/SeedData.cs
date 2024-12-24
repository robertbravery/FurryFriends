using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Infrastructure.Data;

public static class SeedData
{
  public static readonly Contributor Contributor1 = new(Name.Create("Snow", "Frog", "Snow Frog", new NameValidator()).Value);
  public static readonly Contributor Contributor2 = new(Name.Create("Snow", "Dog", "Snow Dog", new NameValidator()).Value);


  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Contributors.AnyAsync()) return; // DB has been seeded

    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    var validator = new PhoneNumberValidator();
    Contributor1.SetPhoneNumber("123-456-7890", validator);
    Contributor2.SetPhoneNumber("987-654-3210", validator);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    await dbContext.SaveChangesAsync();
  }
}
