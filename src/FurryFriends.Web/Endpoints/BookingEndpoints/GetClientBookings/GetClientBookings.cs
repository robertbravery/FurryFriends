using Ardalis.GuardClauses;
using FurryFriends.UseCases.Services.BookingService;
using IBookingService = FurryFriends.UseCases.Services.BookingService.IBookingService;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetClientBookings;

/// <summary>
/// Endpoint for getting bookings for a specific client
/// </summary>
public class GetClientBookings(IBookingService bookingService) : Endpoint<GetClientBookingsRequest, List<ClientBookingResponse>>
{
    private readonly IBookingService _bookingService = bookingService;

    public override void Configure()
    {
        Get(GetClientBookingsRequest.Route);
        AllowAnonymous(); // Or apply appropriate authorization
        Options(x => x.WithName("GetClientBookings_" + Guid.NewGuid().ToString()));
        Description(d => d
            .Produces<List<ClientBookingResponse>>(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(GetClientBookingsRequest request, CancellationToken ct)
    {
        Guard.Against.Null(request, nameof(GetClientBookingsRequest));
        Guard.Against.Default(request.ClientId, nameof(request.ClientId));
        Guard.Against.Default(request.StartDate, nameof(request.StartDate));
        Guard.Against.Default(request.EndDate, nameof(request.EndDate));

        var bookings = await _bookingService.GetClientBookingsAsync(
            request.ClientId,
            request.StartDate,
            request.EndDate);

        var bookingResponses = bookings.Select(b => new ClientBookingResponse
        {
            Id = b.Id,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Status = b.Status.ToString(),
            Price = b.Price
        }).ToList();

        await SendOkAsync(bookingResponses, ct);
    }
}
