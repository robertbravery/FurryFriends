namespace FurryFriends.Web.Endpoints.BookingEndpoints.Create;

public record CreateBookingResponse(Guid BookingId, DateTimeOffset StartDate, DateTimeOffset EndDate);
