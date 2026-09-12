using FurryFriends.BlazorUI.Client.Models.Bookings;

namespace FurryFriends.UnitTests.BlazorUI;

/// <summary>
/// The client-side mirror of the booking state machine. These rules decide which
/// buttons the Booking Management page (/bookings/manage) offers, so they are
/// pinned down here.
/// </summary>
public class BookingStatusRulesTests
{
  [Theory]
  [InlineData("Pending", true)]
  [InlineData("Confirmed", false)]
  [InlineData("InProgress", false)]
  [InlineData("Completed", false)]
  [InlineData("Cancelled", false)]
  [InlineData("NoShow", false)]
  public void CanConfirm_OnlyFromPending(string status, bool expected) =>
    BookingStatusRules.CanConfirm(status).Should().Be(expected);

  [Theory]
  [InlineData("Pending", false)]
  [InlineData("Confirmed", true)]
  [InlineData("InProgress", false)]
  [InlineData("Completed", false)]
  [InlineData("Cancelled", false)]
  [InlineData("NoShow", false)]
  public void CanStart_OnlyFromConfirmed(string status, bool expected) =>
    BookingStatusRules.CanStart(status).Should().Be(expected);

  [Theory]
  [InlineData("Pending", false)]
  [InlineData("Confirmed", false)]
  [InlineData("InProgress", true)]
  [InlineData("Completed", false)]
  [InlineData("Cancelled", false)]
  [InlineData("NoShow", false)]
  public void CanComplete_OnlyFromInProgress(string status, bool expected) =>
    BookingStatusRules.CanComplete(status).Should().Be(expected);

  [Theory]
  [InlineData("Pending", false)]
  [InlineData("Confirmed", true)]
  [InlineData("InProgress", false)]
  [InlineData("Completed", false)]
  [InlineData("Cancelled", false)]
  [InlineData("NoShow", false)]
  public void CanMarkNoShow_OnlyFromConfirmed(string status, bool expected) =>
    BookingStatusRules.CanMarkNoShow(status).Should().Be(expected);

  [Theory]
  [InlineData("Pending", true)]
  [InlineData("Confirmed", true)]
  [InlineData("InProgress", false)]
  [InlineData("Completed", false)]
  [InlineData("Cancelled", false)]
  [InlineData("NoShow", false)]
  public void CanCancel_FromPendingOrConfirmed(string status, bool expected) =>
    BookingStatusRules.CanCancel(status).Should().Be(expected);

  [Theory]
  [InlineData("Completed")]
  [InlineData("Cancelled")]
  [InlineData("NoShow")]
  public void TerminalStatuses_OfferNoActions(string status)
  {
    BookingStatusRules.HasAnyAction(status).Should().BeFalse();
    BookingStatusRules.IsTerminal(status).Should().BeTrue();
  }

  [Theory]
  [InlineData("Pending")]
  [InlineData("Confirmed")]
  [InlineData("InProgress")]
  public void NonTerminalStatuses_OfferAtLeastOneAction(string status)
  {
    BookingStatusRules.HasAnyAction(status).Should().BeTrue();
    BookingStatusRules.IsTerminal(status).Should().BeFalse();
  }

  [Theory]
  [InlineData("pending", "warning")]
  [InlineData("Pending", "warning")]
  [InlineData("Confirmed", "info")]
  [InlineData("InProgress", "info")]
  [InlineData("Completed", "success")]
  [InlineData("Cancelled", "neutral")]
  [InlineData("NoShow", "danger")]
  [InlineData("  Confirmed  ", "info")]
  [InlineData("SomethingNew", "neutral")]
  [InlineData(null, "neutral")]
  [InlineData("", "neutral")]
  public void StatusChipClass_MapsKnownStatuses_AndFallsBackToNeutral(string? status, string expected) =>
    BookingStatusRules.StatusChipClass(status).Should().Be(expected);

  [Fact]
  public void UnknownStatus_IsNotTerminal_AndCannotTransition()
  {
    // An unrecognised status must not accidentally look actionable or terminal.
    BookingStatusRules.HasAnyAction("Weird").Should().BeFalse();
    BookingStatusRules.IsTerminal("Weird").Should().BeFalse();
  }
}
