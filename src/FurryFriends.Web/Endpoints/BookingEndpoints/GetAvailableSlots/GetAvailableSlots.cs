using Ardalis.GuardClauses;
using FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailableSlots;

/// <summary>
/// Endpoint for getting available time slots for a PetWalker on a specific date
/// </summary>
public class GetAvailableSlots(IMediator mediator) : Endpoint<GetAvailableSlotsRequest, GetAvailableSlotsResponse>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetAvailableSlotsRequest.Route);
        AllowAnonymous(); // Or apply appropriate authorization
        Options(x => x.WithName("GetAvailableSlots_" + Guid.NewGuid().ToString()));
        Description(d => d
            .Produces<GetAvailableSlotsResponse>(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(GetAvailableSlotsRequest request, CancellationToken ct)
    {
        Guard.Against.Null(request, nameof(GetAvailableSlotsRequest));
        Guard.Against.Default(request.PetWalkerId, nameof(request.PetWalkerId));
        Guard.Against.Default(request.Date, nameof(request.Date));

        var query = new GetAvailabilityQuery(request.PetWalkerId, request.Date);
        var result = await _mediator.Send(query, ct);

        var availableSlots = result?.AvailableSlots;
        var hasSchedule = result?.HasSchedule ?? false;

        if (availableSlots == null || !availableSlots.Any())
        {
            var emptyResponse = new GetAvailableSlotsResponse
            {
                PetWalkerId = request.PetWalkerId,
                Date = request.Date,
                HasSchedule = hasSchedule,
                AvailableSlots = new List<AvailableSlotResponse>()
            };
            await SendOkAsync(emptyResponse, ct);
            return;
        }

        var response = new GetAvailableSlotsResponse
        {
            PetWalkerId = request.PetWalkerId,
            Date = request.Date,
            HasSchedule = hasSchedule,
            AvailableSlots = availableSlots.Select(slot => new AvailableSlotResponse
            {
                StartTime = slot.Start,
                EndTime = slot.End,
                IsAvailable = true,
                Price = 0, // This would need to be calculated based on PetWalker's hourly rate
                Notes = null
            }).ToList()
        };

        await SendOkAsync(response, ct);
    }
}
