using FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Availability;

/// <summary>
/// Removes the availability override for a single date, restoring the weekly pattern.
/// </summary>
public class DeleteDateException(IMediator mediator)
  : Endpoint<DeleteDateExceptionRequest, Result>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Delete(AvailabilityRoutes.DateExceptionByDate);
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("DeletePetWalkerDateException"));

    Summary(s =>
    {
      s.Summary = "Delete a date exception";
      s.Description = "The walker reverts to their standing weekly availability for that date.";
    });
  }

  public override async Task HandleAsync(DeleteDateExceptionRequest request, CancellationToken ct)
  {
    var result = await _mediator.Send(
        new DeletePetWalkerDateExceptionCommand(request.PetWalkerId, request.Date), ct);

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

    await SendNoContentAsync(ct);
  }
}
