using Ardalis.GuardClauses;
using FurryFriends.UseCases.Services.BookingService;
using IBookingService = FurryFriends.UseCases.Services.BookingService.IBookingService;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.CanBookTimeSlot;

/// <summary>
/// Endpoint for checking if a specific time slot can be booked
/// </summary>
public class CanBookTimeSlot(IBookingService bookingService) : Endpoint<CanBookTimeSlotRequest, bool>
{
    private readonly IBookingService _bookingService = bookingService;

    public override void Configure()
    {
        Get(CanBookTimeSlotRequest.Route);
        AllowAnonymous(); // Or apply appropriate authorization
        Options(x => x.WithName("CanBookTimeSlot_" + Guid.NewGuid().ToString()));
        Description(d => d
            .Produces<bool>(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(CanBookTimeSlotRequest request, CancellationToken ct)
    {
        Guard.Against.Null(request, nameof(CanBookTimeSlotRequest));
        Guard.Against.Default(request.PetWalkerId, nameof(request.PetWalkerId));
        Guard.Against.Default(request.StartTime, nameof(request.StartTime));
        Guard.Against.Default(request.EndTime, nameof(request.EndTime));

        var canBook = await _bookingService.CanBookTimeSlotAsync(
            request.PetWalkerId, 
            request.StartTime, 
            request.EndTime);

        await SendOkAsync(canBook, ct);
    }
}
