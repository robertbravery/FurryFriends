using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.LocationAggregate;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.Infrastructure.Data;

public static class SeedData
{

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Clients.AnyAsync()) return; // DB has been seeded

    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    await SeedData.PopulateUserTestDataAsync(dbContext);
    await SeedData.PopulateClientTestDataAsync(dbContext);

  }

  private static async Task PopulateUserTestDataAsync(AppDbContext dbContext)
  {
    // Create a Country
    var country = new Country("Test Country");
    dbContext.Countries.Add(country);

    // Create a Region
    var region = new Region("Test Region", country.Id);
    dbContext.Regions.Add(region);

    // Create two Localities
    var locality1 = new Locality("Test Locality 1", region.Id) { Id = Guid.Parse("929ccaf2-8c74-49bb-b9a0-ce26db0611ab") };
    var locality2 = new Locality("Test Locality 2", region.Id);
    dbContext.Localities.Add(locality1);
    dbContext.Localities.Add(locality2);


    var validator = new PhoneNumberValidator();
    var phoneNumber1 = await PhoneNumber.Create("027", "011-123-4567");
    var phoneNumber2 = await PhoneNumber.Create("027", "011-123-4567");
    var address1 = Address.Create("123 Test St", "Test City", "Test State", "US", "12345");
    var address2 = Address.Create("456 Test St", "Test City", "Test State", "US", "12345");
    var name1 = Name.Create("Snow", "Frog").Value;
    var name2 = Name.Create("Snow", "Dog").Value;
    var email1 = Email.Create("test@u.com");
    var email2 = Email.Create("test2@u.com");
    var user1 = PetWalker.Create(name1, email1, phoneNumber1, address1);
    var user2 = PetWalker.Create(name2, email2, phoneNumber2, address2);
    dbContext.PetWalkers.AddRange(new List<PetWalker> { user1, user2 });


    // Create two ServiceAreas
    var serviceArea1 = ServiceArea.Create(user1.Id, locality1.Id);
    var serviceArea2 = ServiceArea.Create(user1.Id, locality2.Id);
    dbContext.ServiceAreas.Add(serviceArea1);
    dbContext.ServiceAreas.Add(serviceArea2);


    await dbContext.SaveChangesAsync();

  }

  private static async Task PopulateClientTestDataAsync(AppDbContext dbContext)
  {
    // Create Value Objects
    var name1 = Name.Create("John", "Smith").Value;
    var name2 = Name.Create("Jane", "Doe").Value;

    var email1 = Email.Create("john.smith@example.com");
    var email2 = Email.Create("jane.doe@example.com");

    var phoneNumber1 = await PhoneNumber.Create("027", "555-123-4567");
    var phoneNumber2 = await PhoneNumber.Create("027", "555-987-6543");

    var address1 = Address.Create("789 Pet Lane", "Pet City", "Pet State", "US", "54321");
    var address2 = Address.Create("321 Animal Ave", "Animal City", "Animal State", "US", "98765");

    // Create Clients using the factory method
    var client1 = Client.Create(
        name1,
        email1,
        phoneNumber1,
        address1,
        ClientType.Regular,
        ReferralSource.Website,
        new TimeOnly(9, 0) // 9:00 AM preferred contact time
    );

    var client2 = Client.Create(
        name2,
        email2,
        phoneNumber2,
        address2,
        ClientType.Premium,
        ReferralSource.Other,
        new TimeOnly(14, 0) // 2:00 PM preferred contact time
    );

    dbContext.Clients.AddRange(new List<Client> { client1, client2 });
    await dbContext.SaveChangesAsync();
  }

}
