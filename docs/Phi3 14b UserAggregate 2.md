#Phi3 14b UserAggregate 2.mmd
Here's the dotnet entity for each class in the Mermaid script, along with a sample configuration file. You can create similar entities and configurations for other classes:

**User Entity**
```csharp
public class User : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    [Required] public string Name { get; set; }
    [Required] public string Email { get; set; }
    [Required] public PhoneNumber PhoneNumber { get; set; }
    [Required] public Address Address { get; set; }
    [Required] public List<Photo> Photos { get; set; }
    
    // other properties and methods...
}
```
**UserConfiguration Entity**
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        // other configurations...
    }
}
```
**Name Entity**
```csharp
public class Name : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
}
```
**Address Entity**
```csharp
public class Address : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    [Required] public string Street { get; set; }
    [Required] public string City { get; set; }
    [Required] public string State { get; set; }
    [Required] public string ZipCode { get; set; }
    [Required] public string Country { get; set; }
}
```
**PhoneNumber Entity**
```csharp
public class PhoneNumber : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    [Required] public string CountryCode { get; set; }
    [Required] public string Number { get; set; }
}
```
**Schedule Entity**
```csharp
public class Schedule : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    public List<TimeSlot> RegularSlots { get; set; }
    public List<DateTimeOffset> BlockedDates { get; set; }
}
```
**TimeSlot Entity**
```csharp
public class TimeSlot : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
```
**BackgroundCheck Entity**
```csharp
public class BackgroundCheck : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    [Required] public string ReferenceNumber { get; set; }
    public DateTimeOffset CheckDate { get; set; }
    public int Rating { get; set; }
}
```
**Rating Entity**
```csharp
public class Rating : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    public User User { get; set; }
    public int Score { get; set; } // between 1 and 5 inclusive
}
```
**Testimonial Entity**
```csharp
public class Testimonial : IEntity<Guid>
{
    [Key] public Guid Id { get; set; }
    
    [Required] public User ClientId { get; set; }
    [Required] public string Content { get; set; }
    [Required] public int Rating { get; set; }
    [Required] public DateTimeOffset Date { get; set; }
}
```
**TestimonialConfiguration Entity**
```csharp
public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.HasOne(t => t.ClientId).WithMany().HasForeignKey(t => t.ReferenceNumber);
        
        // other configurations...
    }
}
```
**Note:** The configuration files for each entity should be placed in the corresponding folder structure of your project, and registered in `Startup.cs` using dependency injection with Entity Framework Core's Fluent API. For example:

```csharp
services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("Default")));
    
var builder = services.BuildServiceProvider(). GetRequiredService<ModelBuilder>();
        
// add configurations...
builder.ApplyConfiguration(new UserConfiguration());
```
This should give you a good starting point for creating your Mermaid script and Entity Framework Core configuration files!