using Ardalis.GuardClauses;
using FurryFriends.UseCases.Services.BookingService;
using IBookingService = FurryFriends.UseCases.Services.BookingService.IBookingService;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetPetWalkerBookings;

/// <summary>
/// Endpoint for getting bookings for a specific PetWalker
/// </summary>
public class GetPetWalkerBookings(IBookingService bookingService) : Endpoint<GetPetWalkerBookingsRequest, List<PetWalkerBookingResponse>>
{
    private readonly IBookingService _bookingService = bookingService;

    public override void Configure()
    {
        Get(GetPetWalkerBookingsRequest.Route);
        AllowAnonymous(); // Or apply appropriate authorization
        Options(x => x.WithName("GetPetWalkerBookings_" + Guid.NewGuid().ToString()));
        Description(d => d
            .Produces<List<PetWalkerBookingResponse>>(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(GetPetWalkerBookingsRequest request, CancellationToken ct)
    {
        Guard.Against.Null(request, nameof(GetPetWalkerBookingsRequest));
        Guard.Against.Default(request.PetWalkerId, nameof(request.PetWalkerId));
        Guard.Against.Default(request.StartDate, nameof(request.StartDate));
        Guard.Against.Default(request.EndDate, nameof(request.EndDate));

        var bookings = await _bookingService.GetPetWalkerBookingsAsync(
            request.PetWalkerId,
            request.StartDate,
            request.EndDate);

        var bookingResponses = bookings.Select(b => new PetWalkerBookingResponse
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
