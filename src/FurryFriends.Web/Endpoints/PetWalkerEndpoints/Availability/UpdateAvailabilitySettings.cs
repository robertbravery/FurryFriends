using FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Availability;

/// <summary>
/// Updates the walker's derived-availability settings.
/// </summary>
public class UpdateAvailabilitySettings(IMediator mediator)
  : Endpoint<UpdateAvailabilitySettingsRequest, Result<AvailabilitySettingsResponse>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(AvailabilityRoutes.Settings);
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("UpdatePetWalkerAvailabilitySettings"));

    Summary(s =>
    {
      s.Summary = "Update availability settings";
      s.Description = "Sets slot length, the gap required between bookings, and the daily walk cap.";
    });
  }

  public override async Task HandleAsync(UpdateAvailabilitySettingsRequest request, CancellationToken ct)
  {
    var command = new UpdatePetWalkerAvailabilitySettingsCommand(
        request.PetWalkerId,
        request.SlotDurationMinutes,
        request.InterBookingGapMinutes,
        request.DailyPetWalkLimit);

    var result = await _mediator.Send(command, ct);

    if (!result.IsSuccess)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }

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
