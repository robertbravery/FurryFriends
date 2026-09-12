using FurryFriends.UseCases.Domain.PetWalkers.Query.GetAvailability;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Availability;

/// <summary>
/// Reads the walker's derived-availability settings.
/// </summary>
public class GetAvailabilitySettings(IMediator mediator)
  : Endpoint<GetDateExceptionsRequest, Result<AvailabilitySettingsResponse>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(AvailabilityRoutes.Settings);
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("GetPetWalkerAvailabilitySettings"));

    Summary(s =>
    {
      s.Summary = "Get availability settings";
      s.Description = "Slot length, gap between bookings, and daily walk cap used to derive bookable slots.";
    });
  }

  public override async Task HandleAsync(GetDateExceptionsRequest request, CancellationToken ct)
  {
    var result = await _mediator.Send(
        new GetPetWalkerAvailabilitySettingsQuery(request.PetWalkerId), ct);

    if (!result.IsSuccess)
    {
      if (result.Status == ResultStatus.NotFound)
      {
        await SendNotFoundAsync(ct);
        return;
      }

      await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
      return;
    }

    var dto = result.Value;

    await SendOkAsync(Result.Success(new AvailabilitySettingsResponse
    {
      PetWalkerId = dto.PetWalkerId,
      SlotDurationMinutes = dto.SlotDurationMinutes,
      InterBookingGapMinutes = dto.InterBookingGapMinutes,
      DailyPetWalkLimit = dto.DailyPetWalkLimit
    }), ct);
  }
}
