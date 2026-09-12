namespace FurryFriends.UseCases.Domain.Bookings.Dto;

/// <summary>
/// Result of a booking lifecycle state transition (confirm, start, complete, no-show).
/// </summary>
public record BookingStatusDto(Guid BookingId, string Status, string Message);
