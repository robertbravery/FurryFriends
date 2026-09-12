namespace FurryFriends.Core.PetWalkerAggregate.Services;

/// <summary>
/// A date-scoped override of the weekly availability pattern.
/// Mirrors <see cref="PetWalkerDateException"/> in a form the calculator can consume
/// without taking a dependency on persistence.
/// </summary>
/// <param name="IsUnavailable">When true, no slots are produced for the date at all.</param>
/// <param name="CustomStart">Replacement window start; ignored when <paramref name="IsUnavailable"/>.</param>
/// <param name="CustomEnd">Replacement window end; ignored when <paramref name="IsUnavailable"/>.</param>
public record AvailabilityDateOverride(
    bool IsUnavailable,
    TimeOnly? CustomStart = null,
    TimeOnly? CustomEnd = null);

/// <summary>
/// Derives the bookable slots for a single date from a PetWalker's weekly pattern.
/// <para>
/// This is deliberately a pure function with no persistence or clock dependency: slot
/// derivation is the core availability rule and must be testable in isolation, and every
/// caller (availability query, booking guard) must reach the same answer.
/// </para>
/// </summary>
public static class AvailabilityCalculator
{
  /// <summary>
  /// Calculates the bookable slots for <paramref name="date"/>.
  /// </summary>
  /// <param name="date">The date to calculate for. Only the date component is used.</param>
  /// <param name="weeklyWindows">The walker's standing windows for this weekday.</param>
  /// <param name="slotDurationMinutes">Length of each derived slot, in minutes.</param>
  /// <param name="interBookingGapMinutes">Required gap either side of an existing booking.</param>
  /// <param name="dailyPetWalkLimit">Maximum active bookings per day; stops offering slots once reached.</param>
  /// <param name="activeBookingCount">Count of non-cancelled bookings already on this date.</param>
  /// <param name="existingBookings">
  /// Ranges already taken. Cancelled and no-show bookings must be excluded by the caller.
  /// </param>
  /// <param name="blockedRanges">
  /// Additional unavailable ranges - legacy per-slot rows kept as blocks, local holds, and so on.
  /// </param>
  /// <param name="dateOverride">Optional date-specific override of the weekly pattern.</param>
  public static IReadOnlyList<(DateTime Start, DateTime End)> Calculate(
      DateTime date,
      IEnumerable<(TimeOnly Start, TimeOnly End)> weeklyWindows,
      int slotDurationMinutes,
      int interBookingGapMinutes,
      int dailyPetWalkLimit,
      int activeBookingCount,
      IEnumerable<(DateTime Start, DateTime End)>? existingBookings = null,
      IEnumerable<(DateTime Start, DateTime End)>? blockedRanges = null,
      AvailabilityDateOverride? dateOverride = null)
  {
    var slots = new List<(DateTime Start, DateTime End)>();

    // A whole-day block overrides everything else.
    if (dateOverride is { IsUnavailable: true })
    {
      return slots;
    }

    // Slot duration drives generation; a non-positive value can only yield nothing,
    // so normalise it to the walker default rather than silently returning an empty day.
    if (slotDurationMinutes <= 0)
    {
      slotDurationMinutes = PetWalker.DefaultSlotDurationMinutes;
    }

    if (interBookingGapMinutes < 0)
    {
      interBookingGapMinutes = 0;
    }

    var day = date.Date;

    // A date override with replacement hours supersedes the weekly pattern for that date.
    var windows = dateOverride is { IsUnavailable: false, CustomStart: not null, CustomEnd: not null }
        ? new List<(TimeOnly Start, TimeOnly End)>
          {
            (dateOverride.CustomStart!.Value, dateOverride.CustomEnd!.Value)
          }
        : weeklyWindows.ToList();

    // Normalise the taken ranges. Bookings and blocks are treated identically - both
    // simply mean "this time is not free".
    var occupied = new List<(DateTime Start, DateTime End)>();
    foreach (var (start, end) in existingBookings ?? Enumerable.Empty<(DateTime, DateTime)>())
    {
      if (end > start)
      {
        occupied.Add((start, end));
      }
    }

    foreach (var (start, end) in blockedRanges ?? Enumerable.Empty<(DateTime, DateTime)>())
    {
      if (end > start)
      {
        occupied.Add((start, end));
      }
    }

    // The daily walk limit caps how many more bookings the walker will accept today.
    var remainingCapacity = dailyPetWalkLimit - activeBookingCount;
    if (remainingCapacity <= 0)
    {
      return slots;
    }

    var gap = TimeSpan.FromMinutes(interBookingGapMinutes);
    var slotLength = TimeSpan.FromMinutes(slotDurationMinutes);

    foreach (var window in windows.OrderBy(w => w.Start))
    {
      var windowStart = day.Add(window.Start.ToTimeSpan());
      var windowEnd = day.Add(window.End.ToTimeSpan());

      if (windowEnd <= windowStart)
      {
        continue;
      }

      var slotStart = windowStart;

      while (slots.Count < remainingCapacity)
      {
        var slotEnd = slotStart + slotLength;

        // Never emit a partial slot: it must fit entirely within the window.
        if (slotEnd > windowEnd)
        {
          break;
        }

        // A slot is unusable when it touches an occupied range expanded by the travel gap.
        var isFree = !occupied.Any(o =>
            slotStart < o.End + gap && slotEnd > o.Start - gap);

        if (isFree)
        {
          slots.Add((slotStart, slotEnd));
        }

        slotStart = slotEnd;
      }

      if (slots.Count >= remainingCapacity)
      {
        break;
      }
    }

    return slots;
  }
}
