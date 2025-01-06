using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.UserAggregate;
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
    Contributor Contributor1 = new(Name.Create("Snow", "Frog", new NameValidator()).Value);
    Contributor Contributor2 = new(Name.Create("Snow", "Dog", new NameValidator()).Value);
    var validator = new PhoneNumberValidator();
    await Contributor1.SetPhoneNumber("123-456-7890", validator);
    await Contributor2.SetPhoneNumber("987-654-3210", validator);
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    await dbContext.SaveChangesAsync();

  }

  private static async Task PopulateUserTestDataAsync(AppDbContext dbContext)
  {
    var validator = new PhoneNumberValidator();
    var phoneNumber1 = await PhoneNumber.Create("027", "011-123-4567", validator);
    var phoneNumber2 = await PhoneNumber.Create("027", "011-123-4567", validator);
    var address1 = Address.Create("123 Test St", "Test City", "Test State", "US", "12345");
    var address2 = Address.Create("456 Test St", "Test City", "Test State", "US", "12345");
    var name1 = Name.Create("John", "Doe", new NameValidator()).Value;
    var name2 = Name.Create("Snow", "Dog", new NameValidator()).Value;
    var email1 = Core.ValueObjects.Email.Create("test1@u.com");
    var email2 = Core.ValueObjects.Email.Create("test2@u.com");
    var user1 = PetWalker.Create(name1, email1, phoneNumber1, address1);
    var user2 = PetWalker.Create(name2, email2, phoneNumber2, address2);

    var location1 = ServiceArea.Create(user1.Id, Guid.Parse("df2f290d-d72f-4a7e-9254-8c1aae7ba370"));
    var location2 = ServiceArea.Create(user1.Id, Guid.Parse("44eec69e-c38a-4111-8825-bdc52a9303af"));
    var location3 = ServiceArea.Create(user2.Id, Guid.Parse("df2f290d-d72f-4a7e-9254-8c1aae7ba371"));
    var location4 = ServiceArea.Create(user2.Id, Guid.Parse("44eec69e-c38a-4111-8825-bdc52a9303ae"));

      user1.ServiceAreas.Add(location1);
      user1.ServiceAreas.Add(location2);
      user2.ServiceAreas.Add(location3);
      user2.ServiceAreas.Add(location4);
   

    dbContext.Users.AddRange([user1, user2]);
    dbContext.AddRange([location1, location2, location3, location4]);
    await dbContext.SaveChangesAsync();

  }
}
