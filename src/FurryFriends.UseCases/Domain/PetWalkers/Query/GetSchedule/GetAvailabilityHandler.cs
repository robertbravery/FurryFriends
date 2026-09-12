using Ardalis.GuardClauses;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.Enums;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Services;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

public class GetAvailabilityHandler : IRequestHandler<GetAvailabilityQuery, GetAvailabilityResult>
{
  /// <summary>
  /// Booking statuses that do not hold time. Everything else blocks its range and
  /// counts towards the walker's daily limit, matching the original handler's behaviour.
  /// </summary>
  private static readonly BookingStatus[] NonBlockingStatuses =
  {
    BookingStatus.Cancelled,
    BookingStatus.NoShow
  };

  private readonly IRepository<PetWalker> _walkerRepo;
  private readonly IRepository<Booking> _bookingRepo;
  private readonly IRepository<PetWalkerDateException> _dateExceptionRepo;
  private readonly IRepository<Timeslot> _timeslotRepo;
  private readonly ILogger<GetAvailabilityHandler> _logger;

  public GetAvailabilityHandler(
      IRepository<PetWalker> walkerRepo,
      IRepository<Booking> bookingRepo,
      IRepository<PetWalkerDateException> dateExceptionRepo,
      IRepository<Timeslot> timeslotRepo,
      ILogger<GetAvailabilityHandler> logger)
  {
    _walkerRepo = walkerRepo;
    _bookingRepo = bookingRepo;
    _dateExceptionRepo = dateExceptionRepo;
    _timeslotRepo = timeslotRepo;
    _logger = logger;
  }

  public async Task<GetAvailabilityResult> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
  {
    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken)
        ?? throw new NotFoundException("PetWalker ID", request.PetWalkerId.ToString());

    var dayOfWeek = request.Date.DayOfWeek;
    var dateOnly = DateOnly.FromDateTime(request.Date);

    // A date-specific override can supply hours even when the walker has no standing
    // pattern for this weekday, so resolve it before deciding the day is empty.
    var dateOverride = await LoadDateOverrideAsync(request.PetWalkerId, dateOnly, cancellationToken);

    var weeklyWindows = walker.Schedules
        .Where(s => s.DayOfWeek == dayOfWeek)
        .Select(s => (s.StartTime, s.EndTime))
        .ToList();

    if (dateOverride is not { IsUnavailable: false, CustomStart: not null })
    {
      if (!weeklyWindows.Any())
      {
        _logger.LogInformation(
            "No availability for PetWalker {PetWalkerId} on {Date:yyyy-MM-dd} ({DayOfWeek}): " +
            "walker has {TotalSchedules} schedule(s), none for this weekday",
            request.PetWalkerId, request.Date, dayOfWeek, walker.Schedules.Count);

        return new GetAvailabilityResult
        {
            AvailableSlots = new List<AvailableSlotDto>(),
            HasSchedule = walker.HasAnySchedule
        };
      }
    }

    var bookings = await _bookingRepo.ListAsync(
        new BookingByDateSpec(request.PetWalkerId, request.Date),
        cancellationToken);

    var blockingBookings = bookings
        .Where(b => !NonBlockingStatuses.Contains(b.Status))
        .ToList();

    // Legacy per-slot rows are honoured as blocks so a walker migrating to the weekly
    // pattern cannot be double-booked over an existing per-date slot.
    var blockedRanges = await LoadLegacyBlockedRangesAsync(
        request.PetWalkerId, dateOnly, cancellationToken);

    var slots = AvailabilityCalculator.Calculate(
        date: request.Date,
        weeklyWindows: weeklyWindows,
        slotDurationMinutes: walker.SlotDurationMinutes,
        interBookingGapMinutes: walker.InterBookingGapMinutes,
        dailyPetWalkLimit: walker.DailyPetWalkLimit,
        activeBookingCount: blockingBookings.Count,
        existingBookings: blockingBookings.Select(b => (b.StartTime, b.EndTime)),
        blockedRanges: blockedRanges,
        dateOverride: dateOverride);

    _logger.LogInformation(
        "Availability for PetWalker {PetWalkerId} on {Date:yyyy-MM-dd} ({DayOfWeek}): " +
        "{WindowCount} window(s), {Bookings} active booking(s), {Blocked} blocked period(s), " +
        "{SlotCount} slot(s) of {SlotMinutes}min available",
        request.PetWalkerId, request.Date, dayOfWeek, weeklyWindows.Count,
        blockingBookings.Count, blockedRanges.Count, slots.Count, walker.SlotDurationMinutes);

    return new GetAvailabilityResult
    {
        AvailableSlots = slots.Select(s => new AvailableSlotDto(s.Start, s.End)).ToList(),
        HasSchedule = walker.HasAnySchedule
    };
  }

  private async Task<AvailabilityDateOverride?> LoadDateOverrideAsync(
      Guid petWalkerId,
      DateOnly date,
      CancellationToken cancellationToken)
  {
    var exceptions = await _dateExceptionRepo.ListAsync(
        new PetWalkerDateExceptionByPetWalkerAndDateSpec(petWalkerId, date),
        cancellationToken);

    var exception = exceptions.FirstOrDefault();

    if (exception == null)
    {
      return null;
    }

    return exception.IsUnavailable
        ? new AvailabilityDateOverride(true)
        : new AvailabilityDateOverride(false, exception.StartTime, exception.EndTime);
  }

  /// <summary>
  /// Legacy timeslot rows still hold real time. Booked and unavailable rows are treated
  /// as blocked; available rows are ignored so they don't suppress derived slots.
  /// </summary>
  private async Task<List<(DateTime Start, DateTime End)>> LoadLegacyBlockedRangesAsync(
      Guid petWalkerId,
      DateOnly date,
      CancellationToken cancellationToken)
  {
    var timeslots = await _timeslotRepo.ListAsync(
        new TimeslotsByPetWalkerAndDateSpec(petWalkerId, date),
        cancellationToken);

    return timeslots
        .Where(t => t.Status is TimeslotStatus.Booked or TimeslotStatus.Unavailable)
        .Select(t => (date.ToDateTime(t.StartTime), date.ToDateTime(t.EndTime)))
        .ToList();
  }
}
