namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Client-side mirror of the booking state machine. Determines which lifecycle
/// actions are legal for a booking in a given status. Kept in one place so the
/// rules can be unit tested and stay aligned with the API.
/// </summary>
public static class BookingStatusRules
{
  public const string Pending = "Pending";
  public const string Confirmed = "Confirmed";
  public const string InProgress = "InProgress";
  public const string Completed = "Completed";
  public const string Cancelled = "Cancelled";
  public const string NoShow = "NoShow";

  /// <summary>Pending -&gt; Confirmed</summary>
  public static bool CanConfirm(string? status) => IsStatus(status, Pending);

  /// <summary>Confirmed -&gt; InProgress</summary>
  public static bool CanStart(string? status) => IsStatus(status, Confirmed);

  /// <summary>InProgress -&gt; Completed</summary>
  public static bool CanComplete(string? status) => IsStatus(status, InProgress);

  /// <summary>Confirmed -&gt; NoShow</summary>
  public static bool CanMarkNoShow(string? status) => IsStatus(status, Confirmed);

  /// <summary>Pending or Confirmed -&gt; Cancelled</summary>
  public static bool CanCancel(string? status) => IsStatus(status, Pending) || IsStatus(status, Confirmed);

  /// <summary>True when the status is terminal and no lifecycle action applies.</summary>
  public static bool IsTerminal(string? status) =>
    IsStatus(status, Completed) || IsStatus(status, Cancelled) || IsStatus(status, NoShow);

  public static bool HasAnyAction(string? status) =>
    CanConfirm(status) || CanStart(status) || CanComplete(status) || CanMarkNoShow(status) || CanCancel(status);

  /// <summary>
  /// Maps a booking status onto one of the global <c>.status-chip</c> modifiers
  /// (success / warning / danger / neutral / info). Unknown values fall back to neutral.
  /// </summary>
  public static string StatusChipClass(string? status) => status?.Trim().ToLowerInvariant() switch
  {
    "pending" => "warning",
    "confirmed" => "info",
    "inprogress" => "info",
    "completed" => "success",
    "cancelled" => "neutral",
    "noshow" => "danger",
    _ => "neutral"
  };

  private static bool IsStatus(string? status, string expected) =>
    string.Equals(status?.Trim(), expected, StringComparison.OrdinalIgnoreCase);
}
