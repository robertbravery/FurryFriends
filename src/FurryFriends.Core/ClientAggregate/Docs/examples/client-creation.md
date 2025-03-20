# Client Creation Examples

## Basic Client Creation

```csharp
var client = Client.Create(
    name: Name.Create("John", "Doe"),
    email: Email.Create("john.doe@example.com"),
    phoneNumber: PhoneNumber.Create("+1", "5555555555"),
    address: Address.Create("123 Main St", "Anytown", "CA", "12345", "USA")
);
```

## Premium Client with Preferred Contact Time

```csharp
var premiumClient = Client.Create(
    name: Name.Create("Jane", "Smith"),
    email: Email.Create("jane.smith@example.com"),
    phoneNumber: PhoneNumber.Create("+1", "5555555556"),
    address: Address.Create("456 Oak St", "Somewhere", "NY", "67890", "USA"),
    clientType: ClientType.Premium,
    preferredContactTime: new TimeOnly(14, 0), // 2:00 PM
    referralSource: ReferralSource.ExistingClient
);
```

## Adding a Pet to a Client

```csharp
var pet = Pet.Create(
    name: "Max",
    breedId: 1, // Golden Retriever
    age: 3,
    species: "Dog",
    weight: 30.5,
    color: "Golden",
    ownerId: client.Id
);

client.AddPet(pet);
```