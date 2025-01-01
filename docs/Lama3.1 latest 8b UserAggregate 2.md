#Lama3.1 latest 8b UserAggregate 2.mmd


I'll provide the Entity Framework Core entities for your Clean Architecture core project and the configuration files for each entity in a separate file. I'll follow the structure you specified.

**User.cs**
```csharp
public class User : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
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

    public User(string name, string email, PhoneNumber phone, Address address)
    {
        Name = name;
        Email = email;
        PhoneNumber = phone;
        Address = address;
    }

    // Rest of the methods...
}
```

**Name.cs**
```csharp
public class Name : IEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public bool Validate()
    {
        // Validation logic...
        return true;
    }
}
```

**Address.cs**
```csharp
public class Address : IEntity
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }

    public Result Create(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
        return new Result();
    }

    public bool Validate()
    {
        // Validation logic...
        return true;
    }
}
```

**PhoneNumber.cs**
```csharp
public class PhoneNumber : IEntity
{
    public string CountryCode { get; set; }
    public string Number { get; set; }

    public Result Create(string countryCode, string number)
    {
        CountryCode = countryCode;
        Number = number;
        return new Result();
    }

    public string Format()
    {
        // Format logic...
        return "";
    }

    public bool Validate()
    {
        // Validation logic...
        return true;
    }
}
```

**Schedule.cs**
```csharp
public class Schedule : IEntity
{
    public List<TimeSlot> RegularSlots { get; set; }
    public List<DateTimeOffset> BlockedDates { get; set; }

    public Result AddTimeSlot(TimeSlot timeSlot)
    {
        // Logic...
        return new Result();
    }

    public Result RemoveTimeSlot(TimeSlot timeSlot)
    {
        // Logic...
        return new Result();
    }

    public Result BlockDate(DateTimeOffset date)
    {
        // Logic...
        return new Result();
    }

    public bool IsAvailable(DateTime dateTime, Duration duration)
    {
        // Logic...
        return true;
    }
}
```

**TimeSlot.cs**
```csharp
public class TimeSlot : IEntity
{
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public Result Create(DayOfWeek day, TimeSpan start, TimeSpan end)
    {
        Day = day;
        StartTime = start;
        EndTime = end;
        return new Result();
    }
}
```

**ServiceArea.cs**
```csharp
public class ServiceArea : IEntity
{
    // Properties...
}
```

**BackgroundCheck.cs**
```csharp
public class BackgroundCheck : IEntity
{
    // Properties...
}
```

**Rating.cs**
```csharp
public class Rating : IEntity
{
    // Properties...
}
```

**Testimonial.cs**
```csharp
public class Testimonial : IEntity
{
    // Properties...
}
```

**Badge.cs**
```csharp
public class Badge : IEntity
{
    // Properties...
}
```

**PreferredWorkingHours.cs**
```csharp
public class PreferredWorkingHours : IEntity
{
    // Properties...
}
```

Now, let's create the configuration files for each entity. For example:

**UserConfiguration.cs**
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(255);
        // Other configurations...
    }
}
```

**NameConfiguration.cs**
```csharp
public class NameConfiguration : IEntityTypeConfiguration<Name>
{
    public void Configure(EntityTypeBuilder<Name> builder)
    {
        builder.HasKey(e => new { e.FirstName, e.LastName });
        builder.Property(e => e.FirstName).HasMaxLength(255);
        builder.Property(e => e.LastName).HasMaxLength(255);
        // Other configurations...
    }
}
```

**AddressConfiguration.cs**
```csharp
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(e => new { e.Street, e.City });
        builder.Property(e => e.Street).HasMaxLength(255);
        builder.Property(e => e.City).HasMaxLength(255);
        // Other configurations...
    }
}
```

**PhoneNumberConfiguration.cs**
```csharp
public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
{
    public void Configure(EntityTypeBuilder<PhoneNumber> builder)
    {
        builder.HasKey(e => new { e.CountryCode, e.Number });
        builder.Property(e => e.CountryCode).HasMaxLength(10);
        builder.Property(e => e.Number).HasMaxLength(20);
        // Other configurations...
    }
}
```

**ScheduleConfiguration.cs**
```csharp
public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(e => new { e.Id });
        // Other configurations...
    }
}
```

These are the basic configuration files for each entity. You'll need to create similar files for other entities as well.

Remember to configure the database context and add the configurations to the `DbContext` in your application.

Please let me know if you have any questions or need further assistance!