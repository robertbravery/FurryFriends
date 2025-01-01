#Ollama Areana Model UserAggregate 2.mmd

Certainly! Below are the C# entities and their corresponding configuration classes based on the Mermaid script. Additionally, I'll provide configuration files for each entity where needed.

### Entities

#### `User.cs`
```csharp
using System;
using System.Collections.Generic;

public class User
{
    public Guid Id { get; set; }
    public Name Name { get; set; }
    public string Email { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public Address Address { get; set; }
    public Photo BioPic { get; set; }
    public string Biography { get; set; }
    public List<Photo> Photos { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public bool IsVerified { get; set; }
    public ServiceArea ServiceArea { get; set; }
    public Schedule Availability { get; set; }
    public decimal HourlyRate { get; set; }
    public List<PetType> AcceptedPetTypes { get; set; }
    public int YearsOfExperience { get; set; }
    public bool HasInsurance { get; set; }
    public bool HasFirstAidCertification { get; set; }
    public Rating OverallRating { get; set; }
    public List<Testimonial> Testimonials { get; set; }
    public List<Badge> Badges { get; set; }
    public int MaxPetsPerWalk { get; set; }
    public List<ServiceType> OfferedServices { get; set; }
    public PreferredWorkingHours PreferredHours { get; set; }

    // Constructors and methods
    public User() {}

    public User(string firstName, string lastName, string email, string phoneNumber, Address address, Photo bioPic, string biography)
        : this()
    {
        Name = new Name(firstName, lastName);
        Email = email;
        PhoneNumber = new PhoneNumber("1", phoneNumber); // Default country code
        Address = address;
        BioPic = bioPic;
        Biography = biography;
    }

    public Result<User> Create(string firstName, string lastName, string email, string phoneNumber, Address address, Photo bioPic, string biography)
    {
        // Implementation for creating a user
    }

    // Update methods and other logic
}

// Enumerations
public enum Gender { Male, Female, Other }
public enum ServiceType { PetWalking, DogTraining, Grooming, VetConsultation }
public enum PetType { Cat, Dog, Bird, Fish }
```

#### `Name.cs`
```csharp
public class Name
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        Validate();
    }

    private void Validate()
    {
        // Implementation for validating name fields
    }
}
```

#### `Address.cs`
```csharp
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }

    public Result<Address> Create(string street, string city, string state, string zipCode, string country)
    {
        // Implementation for creating an address
    }

    public bool Validate()
    {
        // Implementation for validating address fields
    }
}
```

#### `PhoneNumber.cs`
```csharp
public class PhoneNumber
{
    public string CountryCode { get; set; }
    public string Number { get; set; }

    public Result<PhoneNumber> Create(string countryCode, string number)
    {
        // Implementation for creating a phone number
    }

    public string Format()
    {
        // Implementation for formatting the phone number
    }

    public bool Validate()
    {
        // Implementation for validating phone number fields
    }
}
```

#### `Schedule.cs`
```csharp
public class Schedule
{
    public List<TimeSlot> RegularSlots { get; set; }
    public List<DateTimeOffset> BlockedDates { get; set; }

    public Result<Schedule> AddTimeSlot(TimeSlot timeSlot)
    {
        // Implementation for adding a time slot
    }

    public Result<Schedule> RemoveTimeSlot(TimeSlot timeSlot)
    {
        // Implementation for removing a time slot
    }
}
```

#### `ServiceArea.cs`
```csharp
public class ServiceArea
{
    public string Description { get; set; }

    // Constructors and methods
    public ServiceArea(string description)
    {
        Description = description;
    }

    // Methods for managing service areas
}
```

#### `BackgroundCheck.cs`
```csharp
public class BackgroundCheck
{
    public bool Completed { get; set; }
    public DateTime CompletionDate { get; set; }

    // Constructors and methods
    public BackgroundCheck()
    {
        Completed = false;
        CompletionDate = DateTime.UtcNow;
    }

    public void Complete(DateTime completionDate)
    {
        Completed = true;
        CompletionDate = completionDate;
    }
}
```

#### `Rating.cs`
```csharp
public class Rating
{
    public int Value { get; set; }
    public DateTime RatingDate { get; set; }

    // Constructors and methods
    public Rating(int value, DateTime ratingDate)
    {
        Value = value;
        RatingDate = ratingDate;
    }
}
```

#### `Testimonial.cs`
```csharp
public class Testimonial
{
    public string Text { get; set; }
    public DateTime Date { get; set; }

    // Constructors and methods
    public Testimonial(string text, DateTime date)
    {
        Text = text;
        Date = date;
    }
}
```

#### `Badge.cs`
```csharp
public class Badge
{
    public string Name { get; set; }
    public DateTime IssuedDate { get; set; }

    // Constructors and methods
    public Badge(string name, DateTime issuedDate)
    {
        Name = name;
        IssuedDate = issuedDate;
    }
}
```

#### `Photo.cs`
```csharp
public class Photo
{
    public string Url { get; set; }
    public string Caption { get; set; }

    // Constructors and methods
    public Photo(string url, string caption)
    {
        Url = url;
        Caption = caption;
    }
}

// Result type for operations
public class Result<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Success = true, Data = data };
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T> { Success = false, ErrorMessage = errorMessage };
    }
}
```

### Configuration Files

For each entity, you would typically have a configuration file to map database schema or application settings. Here are examples for some entities:

#### `UserConfiguration.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class UserConfiguration : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add configuration from appsettings.json or other sources
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }
}
```

#### `ScheduleConfiguration.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class ScheduleConfiguration : DbContext
{
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add configuration from appsettings.json or other sources
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }
}
```

These files should be placed in the appropriate directory of your project and configured according to your application's requirements.