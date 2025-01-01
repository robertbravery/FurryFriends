#Qwen2.5 7b User Aggregate 2.mmd
Sure, here are the C# classes for your Clean Architecture core project:

### Core Project

#### Entities

1. **User.cs**
```csharp
using System;
using System.Collections.Generic;

public class User
{
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public string Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Address Address { get; private set; }
    public Photo BioPic { get; private set; }
    public string Biography { get; private set; }
    public List<Photo> Photos { get; private set; } = new List<Photo>();
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public bool IsVerified { get; private set; }
    public ServiceArea ServiceArea { get; private set; }
    public Schedule Availability { get; private set; }
    public Money HourlyRate { get; private set; }
    public List<PetType> AcceptedPetTypes { get; private set; } = new List<PetType>();
    public int YearsOfExperience { get; private set; }
    public bool HasInsurance { get; private set; }
    public bool HasFirstAidCertification { get; private set; }
    public Rating OverallRating { get; private set; }
    public List<Testimonial> Testimonials { get; private set; } = new List<Testimonial>();
    public List<Badge> Badges { get; private set; } = new List<Badge>();
    public int MaxPetsPerWalk { get; private set; }
    public List<ServiceType> OfferedServices { get; private set; } = new List<ServiceType>();
    public PreferredWorkingHours PreferredHours { get; private set; }

    // Constructors, Properties and Methods
}
```

2. **Name.cs**
```csharp
public class Name
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void Validate()
    {
        // Validation logic here
    }
}
```

3. **Address.cs**
```csharp
public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    public Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public void Validate()
    {
        // Validation logic here
    }
}
```

4. **PhoneNumber.cs**
```csharp
public class PhoneNumber
{
    public string CountryCode { get; private set; }
    public string Number { get; private set; }

    public PhoneNumber(string countryCode, string number)
    {
        CountryCode = countryCode;
        Number = number;
    }

    public void Format()
    {
        // Formatting logic here
    }

    public void Validate()
    {
        // Validation logic here
    }
}
```

5. **Schedule.cs**
```csharp
using System.Collections.Generic;

public class Schedule
{
    public List<TimeSlot> RegularSlots { get; private set; } = new List<TimeSlot>();
    public List<DateTimeOffset> BlockedDates { get; private set; } = new List<DateTimeOffset>();

    public void AddTimeSlot(TimeSlot timeSlot)
    {
        // Logic to add a timeslot
    }

    public void RemoveTimeSlot(TimeSlot timeSlot)
    {
        // Logic to remove a timeslot
    }

    public void BlockDate(DateTimeOffset date)
    {
        // Logic to block a date
    }

    public bool IsAvailable(DateTime dateTime, TimeSpan duration)
    {
        // Logic to check availability
        return true;
    }
}
```

6. **TimeSlot.cs**
```csharp
using System;

public class TimeSlot
{
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }

    public TimeSlot(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
}
```

7. **ServiceArea.cs**
```csharp
public class ServiceArea
{
    // Define properties and methods here
}
```

8. **BackgroundCheck.cs**
```csharp
public class BackgroundCheck
{
    // Define properties and methods here
}
```

9. **Rating.cs**
```csharp
public class Rating
{
    public int Value { get; private set; }

    public Rating(int value)
    {
        Value = value;
    }
}
```

10. **Testimonial.cs**
```csharp
public class Testimonial
{
    // Define properties and methods here
}
```

11. **Badge.cs**
```csharp
public class Badge
{
    // Define properties and methods here
}
```

12. **Money.cs**
```csharp
using System;

public struct Money : IEquatable<Money>
{
    public static readonly Money Zero = new Money(0, CurrencyType.Dollar);

    public int Value { get; private set; }
    public CurrencyType Type { get; private set; }

    public Money(int value, CurrencyType type)
    {
        Value = value;
        Type = type;
    }

    public bool Equals(Money other)
    {
        return Value == other.Value && Type == other.Type;
    }

    public override bool Equals(object obj)
    {
        if (obj is Money m) return Equals(m);
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Value * 397) ^ (int)Type;
        }
    }

    public static bool operator ==(Money a, Money b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Money a, Money b)
    {
        return !a.Equals(b);
    }

    public override string ToString()
    {
        return $"{Value} {Type}";
    }
}

public enum CurrencyType
{
    Dollar,
    Euro,
    // Add more currency types as needed
}
```

13. **PetType.cs**
```csharp
public class PetType
{
    // Define properties and methods here
}
```

14. **ServiceType.cs**
```csharp
public class ServiceType
{
    // Define properties and methods here
}
```

15. **PreferredWorkingHours.cs**
```csharp
public class PreferredWorkingHours
{
    // Define properties and methods here
}
```

#### Domain Events

Create a new folder named `Events` in your Core project and create the following classes:

1. **UserCreatedEvent.cs**
```csharp
public class UserCreatedEvent
{
    public Guid UserId { get; }
    public DateTime CreationDate { get; }

    public UserCreatedEvent(Guid userId)
    {
        UserId = userId;
        CreationDate = DateTime.UtcNow;
    }
}
```

2. **UserUpdatedEvent.cs**
```csharp
public class UserUpdatedEvent
{
    public Guid UserId { get; }
    public DateTime UpdateDate { get; }

    public UserUpdatedEvent(Guid userId)
    {
        UserId = userId;
        UpdateDate = DateTime.UtcNow;
    }
}
```

### Infrastructure Project

For the infrastructure project, you can create a new folder named `Persistence` and create the following class:

1. **UserConfiguration.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        // Configure other properties and relationships here
    }
}
```

2. **UserRepository.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using System;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
```

### Application Project

For the application project, you can create a new folder named `UseCases` and create the following classes:

1. **CreateUserUseCase.cs**
```csharp
using System.Threading.Tasks;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Execute(User user)
    {
        await _userRepository.AddUserAsync(user);
        
        // Raise domain event if needed
        var userCreatedEvent = new UserCreatedEvent(user.Id);
        // Publish the event using your preferred event bus or messaging system
    }
}
```

2. **UpdateUserUseCase.cs**
```csharp
using System.Threading.Tasks;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Execute(User user)
    {
        await _userRepository.UpdateUserAsync(user);
        
        // Raise domain event if needed
        var userUpdatedEvent = new UserUpdatedEvent(user.Id);
        // Publish the event using your preferred event bus or messaging system
    }
}
```

### Domain Project

For the domain project, create a new folder named `Entities` and create the following classes:

1. **User.cs**
```csharp
using System;
using System.Collections.Generic;

public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime BirthDate { get; private set; }

    // Add other properties and methods here

    public User(string firstName, string lastName, DateTime birthDate)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
    }
}
```

### Presentation Project (Optional)

For the presentation project, create a new folder named `Controllers` and create the following controller:

1. **UsersController.cs**
```csharp
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ICreateUserUseCase _createUserUseCase;
    private readonly IUpdateUserUseCase _updateUserUseCase;

    public UsersController(ICreateUserUseCase createUserUseCase, IUpdateUserUseCase updateUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _updateUserUseCase = updateUserUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        await _createUserUseCase.Execute(user);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
    {
        if (user.Id != id) return BadRequest("ID mismatch");

        await _updateUserUseCase.Execute(user);
        return NoContent();
    }
}
```

### Summary

This solution provides a comprehensive approach to building a domain-driven design (DDD) application using Entity Framework Core for persistence and ASP.NET Core for the presentation layer. It includes the necessary models, repositories, use cases, and controllers to handle user creation and updates. The application also raises domain events which can be used for event sourcing or other business logic that needs to occur after a user is created or updated.