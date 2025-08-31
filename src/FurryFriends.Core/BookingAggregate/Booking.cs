using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.BookingAggregate.Events;
using FurryFriends.Core.BookingAggregate.Validation;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.Common;
using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.Core.BookingAggregate;

public partial class Booking : AuditableEntity<Guid>
{
  public Guid PetWalkerId { get; private set; }
  public Guid PetOwnerId { get; private set; }
  public DateTime StartTime { get; private set; }
  public DateTime EndTime { get; private set; }
  public BookingStatus Status { get; private set; }
  public decimal Price { get; private set; }
  public string? Notes { get; private set; }

  public virtual PetWalker PetWalker { get; private set; } = default!;
  public virtual Client PetOwner { get; private set; } = default!;

  internal Booking() { } // Required by EF Core

  private Booking(Guid petWalkerId, Guid petOwnerId, DateTime startTime, DateTime endTime, decimal price)
  {
    Id = Guid.NewGuid();
    PetWalkerId = petWalkerId;
    PetOwnerId = petOwnerId;
    StartTime = startTime;
    EndTime = endTime;
    Price = price;
    Status = BookingStatus.Pending;
  }

  public static Booking Create(
      Guid petOwnerId,
      DateTime startTime,
      DateTime endTime,
      decimal price,
      PetWalker petWalker,
      IEnumerable<Booking> existingBookings)
  {
    Guard.Against.Null(petWalker, nameof(petWalker));
    Guard.Against.Default(petOwnerId, nameof(petOwnerId));
    Guard.Against.OutOfRange(startTime, nameof(startTime), DateTime.UtcNow, DateTime.MaxValue);
    Guard.Against.OutOfRange(endTime, nameof(endTime), startTime, DateTime.MaxValue);
    Guard.Against.NegativeOrZero(price, nameof(price));
    Guard.Against.Null(existingBookings, nameof(existingBookings));

    // Validate against pet walker's schedule
    Guard.Against.InvalidInput(startTime, nameof(startTime),
        _ => petWalker.Schedules.Any(),
        "Pet walker has no available schedule");

    Guard.Against.BookingSchedule(startTime, endTime, petWalker, nameof(startTime));

    // Validate no overlapping bookings
    Guard.Against.NoOverlappingBookings(startTime, endTime, existingBookings, nameof(startTime));

    // Validate daily booking limit
    Guard.Against.DailyBookingLimit(startTime, petWalker, existingBookings, nameof(startTime));

    var booking = new Booking(petWalker.Id, petOwnerId, startTime, endTime, price);
    booking.RegisterDomainEvent(new BookingCreatedEvent(booking));

    return booking;
  }

  public void Confirm()
  {
    if (Status != BookingStatus.Pending)
    {
      throw new InvalidOperationException($"Cannot confirm booking in {Status} status");
    }

    Status = BookingStatus.Confirmed;
    RegisterDomainEvent(new BookingConfirmedEvent(this));
  }

  public void BeginWalk()
  {
    if (Status != BookingStatus.Confirmed)
    {
      throw new InvalidOperationException($"Cannot start booking in {Status} status");
    }

    Status = BookingStatus.InProgress;
    RegisterDomainEvent(new BookingStartedEvent(this));
  }

  public void Complete()
  {
    if (Status != BookingStatus.InProgress)
    {
      throw new InvalidOperationException($"Cannot complete booking in {Status} status");
    }

    Status = BookingStatus.Completed;
    RegisterDomainEvent(new BookingCompletedEvent(this));
  }

  public void Cancel(string? reason = null)
  {
    if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
    {
      throw new InvalidOperationException($"Cannot cancel booking in {Status} status");
    }

    Status = BookingStatus.Cancelled;
    Notes = reason;
    RegisterDomainEvent(new BookingCancelledEvent(this));
  }

  public void MarkAsNoShow()
  {
    if (Status != BookingStatus.Confirmed)
    {
      throw new InvalidOperationException($"Cannot mark as no-show booking in {Status} status");
    }

    Status = BookingStatus.NoShow;
    RegisterDomainEvent(new BookingNoShowEvent(this));
  }

  public void UpdateNotes(string notes)
  {
    Notes = Guard.Against.NullOrEmpty(notes, nameof(notes));
  }

  public bool IsOverlapping(DateTime start, DateTime end)
  {
    return StartTime < end && EndTime > start;
  }

  public TimeSpan Duration => EndTime - StartTime;
}
