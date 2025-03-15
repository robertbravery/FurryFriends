using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UnitTests.TestHelpers;

public static class ClientTestHelpers
{

    private static readonly string[] _firstNames =
    [
            "James", "William", "Oliver", "Henry", "Theodore",
            "Charlotte", "Ava", "Amelia", "Emma", "Sophia"
    ];

    private static readonly string[] _lastNames =
    [
            "Smith", "Johnson", "Williams", "Brown", "Jones",
            "Garcia", "Miller", "Davis", "Rodriguez", "Martinez"
    ];


    public static async Task<Client> CreateTestClientAsync(
        Guid? id = null,
        string firstName = "John",
        string lastName = "Doe",
        string email = "john.doe@example.com",
        string countryCode = "027",
        string phoneNumber = "555-1234",
        string street = "123 Main St",
        string city = "AnyCity",
        string state = "CA",
        string country = "USA",
        string zipCode = "12345",
        ClientType clientType = ClientType.Regular,
        TimeOnly? preferredContactTime = null,
        string referralSource = "Website")
    {
        var client = Client.Create(
            Name.Create(firstName, lastName),
            Email.Create(email),
            await PhoneNumber.Create(countryCode, phoneNumber),
            Address.Create(street, city, state, country, zipCode),
            clientType,
            preferredContactTime ?? new TimeOnly(9, 0),
            referralSource);

        if (id.HasValue)
        {
            // Use reflection to set the Id since it's likely protected
            typeof(Client)
                .GetProperty("Id")?
                .SetValue(client, id.Value);
        }

        return client;
    }

    public static List<Client> CreateTestClients(int count = 3)
    {

        var clients = new List<Client>();
        for (int i = 0; i < count; i++)
        {
            clients.Add(CreateTestClientAsync(
                firstName: _firstNames[i % _firstNames.Length],
                lastName: _lastNames[i % _lastNames.Length],
                email: $"test{i}@example.com"
            ).Result);
        }
        return clients;

    }

    public static Client CreatePremiumTestClient()
    {
        return CreateTestClientAsync(
            firstName: "Premium",
            lastName: "User",
            email: "premium@example.com",
            clientType: ClientType.Premium,
            preferredContactTime: new TimeOnly(14, 0),
            referralSource: "Referral"
        ).Result;
    }
}