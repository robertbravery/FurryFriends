
using Ardalis.GuardClauses;
using Ardalis.Specification;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.BookingAggregate.Specifications;
using FurryFriends.Core.BookingAggregate.Validation;
using FurryFriends.Core.PetWalkerAggregate;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Services.BookingService;

public class BookingService : IBookingService
{
  private readonly IRepository<Booking> _bookingRepository;
  private readonly IRepository<PetWalker> _petWalkerRepository;
  private readonly ILogger<BookingService> _logger;

  public BookingService(
      IRepository<Booking> bookingRepository,
      IRepository<PetWalker> petWalkerRepository,
      ILogger<BookingService> logger)
  {
    _bookingRepository = bookingRepository;
    _petWalkerRepository = petWalkerRepository;
    _logger = logger;
  }

  public async Task<Booking> CreateBookingAsync(
      Guid petWalkerId,
      Guid clientId,
      DateTime startTime,
      DateTime endTime,
      decimal price)
  {
    var petWalker = await _petWalkerRepository.GetByIdAsync(petWalkerId, CancellationToken.None);
    Guard.Against.NotFound(petWalkerId.ToString(), petWalker, nameof(petWalkerId));

    var existingBookings = await GetPetWalkerBookingsAsync(petWalkerId, startTime, endTime);

    Guard.Against.Null(petWalker, nameof(petWalker));
    Guard.Against.BookingSchedule(startTime, endTime, petWalker, nameof(startTime));
    Guard.Against.NoOverlappingBookings(startTime, endTime, existingBookings, nameof(startTime));
    Guard.Against.DailyBookingLimit(startTime, petWalker, existingBookings, nameof(startTime));

    var booking = Booking.Create(
        clientId,
        startTime,
        endTime,
        price,
        petWalker,
        existingBookings);

    await _bookingRepository.AddAsync(booking, CancellationToken.None);

    _logger.LogInformation(
        "Created booking {BookingId} for pet walker {PetWalkerId} and client {ClientId}",
        booking.Id, petWalkerId, clientId);

    return booking;
  }

  public async Task<Booking> GetBookingAsync(Guid bookingId)
  {
    var booking = await _bookingRepository.GetByIdAsync(bookingId, CancellationToken.None);
    Guard.Against.NotFound(bookingId.ToString(), booking, nameof(bookingId));

    return booking;
  }

  public async Task<IEnumerable<Booking>> GetPetWalkerBookingsAsync(
      Guid petWalkerId,
      DateTime startTime,
      DateTime endTime)
  {
    var spec = new BookingsByPetWalkerSpec(petWalkerId, startTime, endTime);
    var existingBookings = await _bookingRepository.ListAsync(spec, CancellationToken.None);
    return existingBookings;
  }

  public async Task<IEnumerable<Booking>> GetClientBookingsAsync(
      Guid clientId,
      DateTime startDate,
      DateTime endDate)
  {
    var spec = new BookingsByClientSpec(clientId, startDate, endDate);
    return await _bookingRepository.ListAsync(spec, CancellationToken.None);
  }

  public async Task<Booking> UpdateBookingStatusAsync(
      Guid bookingId,
      BookingStatus newStatus,
      string? notes = null)
  {
    var booking = await _bookingRepository.GetByIdAsync(bookingId, CancellationToken.None);
    Guard.Against.NotFound(bookingId.ToString(), booking, nameof(bookingId));

    if (booking is null)
    {
      throw new ArgumentException($"Booking with id {bookingId} not found.");
    }

    switch (newStatus)
    {
      case BookingStatus.Confirmed:
        booking.Confirm();
        break;
      case BookingStatus.InProgress:
        booking.BeginWalk();
        break;
      case BookingStatus.Completed:
        booking.Complete();
        break;
      case BookingStatus.Cancelled:
        booking.Cancel(notes);
        break;
      case BookingStatus.NoShow:
        booking.MarkAsNoShow();
        break;
      default:
        throw new ArgumentException($"Invalid booking status: {newStatus}", nameof(newStatus));
    }

    await _bookingRepository.UpdateAsync(booking, CancellationToken.None);

    _logger.LogInformation(
        "Updated booking {BookingId} status to {Status}",
        bookingId, newStatus);

    return booking;
  }

  public async Task<bool> CanBookTimeSlotAsync(
      Guid petWalkerId,
      DateTime startTime,
      DateTime endTime)
  {
    var petWalker = await _petWalkerRepository.GetByIdAsync(petWalkerId, CancellationToken.None);
    if (petWalker == null) return false;

    try
    {
      var existingBookings = await GetPetWalkerBookingsAsync(petWalkerId, startTime, endTime);

      Guard.Against.BookingSchedule(startTime, endTime, petWalker, nameof(startTime));
      Guard.Against.NoOverlappingBookings(startTime, endTime, existingBookings, nameof(startTime));
      Guard.Against.DailyBookingLimit(startTime, petWalker, existingBookings, nameof(startTime));

      return true;
    }
    catch
    {
      return false;
    }
  }

  public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(
      Guid petWalkerId,
      DateTime date)
  {
    var petWalker = await _petWalkerRepository.GetByIdAsync(petWalkerId, CancellationToken.None);
    Guard.Against.NotFound(petWalkerId.ToString(), petWalker, nameof(petWalkerId));

    var schedules = petWalker.Schedules
        .Where(s => s.DayOfWeek == date.DayOfWeek)
        .ToList();

    if (schedules is null || !schedules.Any())
    {
      return Enumerable.Empty<TimeSlot>();
    }

    var existingBookings = await GetPetWalkerBookingsAsync(petWalkerId, date, date.AddDays(1));

    var slots = new List<TimeSlot>();

    foreach (var schedule in schedules)
    {
      var currentStart = new DateTime(
          date.Year, date.Month, date.Day,
          schedule.StartTime.Hour, schedule.StartTime.Minute, 0);
      var dayEnd = new DateTime(
          date.Year, date.Month, date.Day,
          schedule.EndTime.Hour, schedule.EndTime.Minute, 0);

      // Create 1-hour slots
      while (currentStart.AddHours(1) <= dayEnd)
      {
        var slotEnd = currentStart.AddHours(1);

        // Check if slot overlaps with any existing booking
        if (!existingBookings.Any(b => b.StartTime < slotEnd && b.EndTime > currentStart))
        {
          slots.Add(new TimeSlot(currentStart, slotEnd));
        }

        currentStart = slotEnd;
      }
    }

    return slots;
  }
}
