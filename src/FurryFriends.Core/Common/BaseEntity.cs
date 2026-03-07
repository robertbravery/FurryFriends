// Core/Entities/Booking.cs
public class BaseEntity<T>
{
  public required T Id { get; set; }

  public BaseEntity(T id)
  {
    Id = id;
  }

  protected BaseEntity() { } // For EF Core
}
