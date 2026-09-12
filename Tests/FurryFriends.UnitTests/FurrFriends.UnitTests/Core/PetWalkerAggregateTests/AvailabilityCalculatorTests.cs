using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Services;

namespace FurryFriends.UnitTests.Core.PetWalkerAggregateTests;

/// <summary>
/// The availability calculator is the single source of truth for what a PetWalker can be
/// booked for. These tests pin down every rule so the derived-slot behaviour cannot drift
/// silently once the per-date timeslot path is no longer the primary flow.
/// </summary>
public class AvailabilityCalculatorTests
{
  private static readonly DateTime Monday = new(2026, 9, 14); // a Monday

  private static (TimeOnly Start, TimeOnly End) Window(int startHour, int endHour) =>
    (new TimeOnly(startHour, 0), new TimeOnly(endHour, 0));

  [Fact]
  public void Calculate_WithNoWindows_ReturnsNoSlots()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        weeklyWindows: Array.Empty<(TimeOnly, TimeOnly)>(),
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().BeEmpty();
  }

  [Fact]
  public void Calculate_SplitsWindowIntoSlotsOfTheConfiguredDuration()
  {
    // 09:00-12:00 at 60 minutes => 09-10, 10-11, 11-12
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 12) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().HaveCount(3);
    slots[0].Start.Should().Be(Monday.AddHours(9));
    slots[0].End.Should().Be(Monday.AddHours(10));
    slots[2].Start.Should().Be(Monday.AddHours(11));
    slots[2].End.Should().Be(Monday.AddHours(12));
  }

  [Fact]
  public void Calculate_WithThirtyMinuteSlots_ProducesTwiceAsManySlots()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 11) },
        slotDurationMinutes: 30,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().HaveCount(4);
    slots[0].Start.Should().Be(Monday.AddHours(9));
    slots[3].End.Should().Be(Monday.AddHours(11));
  }

  [Fact]
  public void Calculate_DropsPartialTrailingSlot_WhenWindowDoesNotDivideEvenly()
  {
    // 09:00-10:50 at 60 minutes => only 09:00-10:00 fits; 10:00-11:00 would exceed the window
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { (new TimeOnly(9, 0), new TimeOnly(10, 50)) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().HaveCount(1);
    slots[0].End.Should().Be(Monday.AddHours(10));
  }

  [Fact]
  public void Calculate_HandlesMultipleWindowsOnTheSameDay()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 10), Window(14, 16) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().HaveCount(3);
    slots.Select(s => s.Start.Hour).Should().Equal(9, 14, 15);
  }

  [Fact]
  public void Calculate_ExcludesSlotOverlappingAnExistingBooking()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 12) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 1,
        existingBookings: new[] { (Monday.AddHours(10), Monday.AddHours(11)) });

    slots.Select(s => s.Start.Hour).Should().Equal(9, 11);
  }

  [Fact]
  public void Calculate_ExcludesSlotsWithinTheInterBookingGap()
  {
    // Booking 10:00-11:00 with a 30 minute gap should also disallow 09:30-10:30
    // and 11:00-12:00 style slots that touch the gap.
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 13) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 30,
        dailyPetWalkLimit: 8,
        activeBookingCount: 1,
        existingBookings: new[] { (Monday.AddHours(10), Monday.AddHours(11)) });

    // 09-10 overlaps the gap before the booking, 10-11 is the booking,
    // 11-12 overlaps the gap after, so only 12-13 remains.
    slots.Select(s => s.Start.Hour).Should().Equal(12);
  }

  [Fact]
  public void Calculate_WithZeroGap_AllowsSlotAdjacentToABooking()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 12) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 1,
        existingBookings: new[] { (Monday.AddHours(10), Monday.AddHours(11)) });

    slots.Select(s => s.Start.Hour).Should().Equal(9, 11);
  }

  [Fact]
  public void Calculate_StopsOfferingSlots_WhenDailyLimitReached()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 17) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 2,
        activeBookingCount: 2);

    slots.Should().BeEmpty();
  }

  [Fact]
  public void Calculate_CapsSlotsAtRemainingDailyCapacity()
  {
    // Limit 4, one already booked => at most 3 more slots offered
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 17) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 4,
        activeBookingCount: 1,
        existingBookings: new[] { (Monday.AddHours(9), Monday.AddHours(10)) });

    slots.Should().HaveCount(3);
  }

  [Fact]
  public void Calculate_WholeDayOverride_ReturnsNoSlots_EvenWithWeeklyWindows()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 17) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0,
        dateOverride: new AvailabilityDateOverride(IsUnavailable: true));

    slots.Should().BeEmpty();
  }

  [Fact]
  public void Calculate_CustomHoursOverride_ReplacesTheWeeklyWindows()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 17) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0,
        dateOverride: new AvailabilityDateOverride(
            IsUnavailable: false,
            CustomStart: new TimeOnly(13, 0),
            CustomEnd: new TimeOnly(15, 0)));

    slots.Should().HaveCount(2);
    slots.Select(s => s.Start.Hour).Should().Equal(13, 14);
  }

  [Fact]
  public void Calculate_CustomHoursOverride_WorksWhenNoWeeklyWindowExists()
  {
    // A walker with no standing Monday pattern can still open a one-off Monday window.
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        weeklyWindows: Array.Empty<(TimeOnly, TimeOnly)>(),
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0,
        dateOverride: new AvailabilityDateOverride(
            IsUnavailable: false,
            CustomStart: new TimeOnly(10, 0),
            CustomEnd: new TimeOnly(11, 0)));

    slots.Should().HaveCount(1);
    slots[0].Start.Should().Be(Monday.AddHours(10));
  }

  [Fact]
  public void Calculate_SubtractsBlockedRanges_SoLegacyTimeslotsCannotBeDoubleBooked()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 12) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0,
        blockedRanges: new[] { (Monday.AddHours(10), Monday.AddHours(11)) });

    slots.Select(s => s.Start.Hour).Should().Equal(9, 11);
    slots.Select(s => s.Start.Hour).Should().NotContain(10);
  }

  [Fact]
  public void Calculate_IgnoresInvertedOrEmptyRanges()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 11) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0,
        existingBookings: new[] { (Monday.AddHours(15), Monday.AddHours(15)) },
        blockedRanges: new[] { (Monday.AddHours(16), Monday.AddHours(15)) });

    slots.Should().HaveCount(2);
  }

  [Fact]
  public void Calculate_IgnoresWindowsOutsideTheRequestedDate()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 11) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().OnlyContain(s => s.Start.Date == Monday.Date && s.End.Date == Monday.Date);
  }

  [Fact]
  public void Calculate_NormalisesNonPositiveSlotDuration_InsteadOfReturningNothing()
  {
    // A legacy row written before the default existed could carry 0. Returning an empty
    // day would make the walker silently unbookable, so the default is applied instead.
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 12) },
        slotDurationMinutes: 0,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().HaveCount(3);
    slots[0].End.Should().Be(Monday.AddHours(10));
  }

  [Fact]
  public void Calculate_NormalisesNegativeGap_ToZero()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 12) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: -30,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0,
        existingBookings: new[] { (Monday.AddHours(10), Monday.AddHours(11)) });

    slots.Select(s => s.Start.Hour).Should().Equal(9, 11);
  }

  [Fact]
  public void Calculate_SkipsDegenerateWindow_WhereEndIsNotAfterStart()
  {
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { (new TimeOnly(9, 0), new TimeOnly(9, 0)), Window(14, 15) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 8,
        activeBookingCount: 0);

    slots.Should().HaveCount(1);
    slots[0].Start.Should().Be(Monday.AddHours(14));
  }

  [Fact]
  public void Calculate_ReturnsNoSlots_WhenLimitIsZero()
  {
    // Mirrors the guard in BookingValidationRules.ValidateDailyBookingLimit where
    // bookingsOnDate >= DailyPetWalkLimit rejects the booking.
    var slots = AvailabilityCalculator.Calculate(
        Monday,
        new[] { Window(9, 17) },
        slotDurationMinutes: 60,
        interBookingGapMinutes: 0,
        dailyPetWalkLimit: 0,
        activeBookingCount: 0);

    slots.Should().BeEmpty();
  }
}
