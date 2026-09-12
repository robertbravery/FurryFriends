using FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Availability;

/// <summary>
/// Creates or replaces the availability override for a single date.
/// One exception per walker per date, so this is an upsert.
/// </summary>
public class SetDateException(IMediator mediator)
  : Endpoint<SetDateExceptionRequest, Result<DateExceptionResponse>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AvailabilityRoutes.DateExceptions);
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("SetPetWalkerDateException"));

    Summary(s =>
    {
      s.Summary = "Set a date exception for a PetWalker";
      s.Description = "Blocks a whole day, or replaces the hours for that day, without touching the weekly pattern.";
    });
  }

  public override async Task HandleAsync(SetDateExceptionRequest request, CancellationToken ct)
  {
    if (request.Date == default)
    {
      AddError(nameof(request.Date), "Date is required");
      await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
      return;
    }

    if (!request.IsUnavailable && (request.StartTime == null || request.EndTime == null))
    {
      AddError(nameof(request.StartTime), "Start and end time are required when the day is not fully blocked");
      await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
      return;
    }

    var command = new SetPetWalkerDateExceptionCommand(
        request.PetWalkerId,
        request.Date,
        request.IsUnavailable,
        request.StartTime,
        request.EndTime,
        request.Reason);

    var result = await _mediator.Send(command, ct);

    if (!result.IsSuccess)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }

      await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
      return;
    }

    var dto = result.Value;

    await SendOkAsync(Result.Success(new DateExceptionResponse
    {
      Id = dto.Id,
      PetWalkerId = dto.PetWalkerId,
      Date = dto.Date,
      IsUnavailable = dto.IsUnavailable,
      StartTime = dto.StartTime,
      EndTime = dto.EndTime,
      Reason = dto.Reason
    }), ct);
  }
}
