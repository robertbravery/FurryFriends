using FurryFriends.UseCases.Domain.PetWalkers.Query.GetAvailability;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Availability;

/// <summary>
/// Lists the date-specific availability overrides for a PetWalker.
/// </summary>
public class GetDateExceptions(IMediator mediator)
  : Endpoint<GetDateExceptionsRequest, Result<List<DateExceptionResponse>>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(AvailabilityRoutes.DateExceptions);
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("GetPetWalkerDateExceptions"));

    Summary(s =>
    {
      s.Summary = "List date exceptions for a PetWalker";
      s.Description = "Returns every date on which the walker's weekly availability is overridden.";
    });
  }

  public override async Task HandleAsync(GetDateExceptionsRequest request, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetPetWalkerDateExceptionsQuery(request.PetWalkerId), ct);

    if (!result.IsSuccess)
    {
      await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
      return;
    }

    var response = result.Value.Select(e => new DateExceptionResponse
    {
      Id = e.Id,
      PetWalkerId = e.PetWalkerId,
      Date = e.Date,
      IsUnavailable = e.IsUnavailable,
      StartTime = e.StartTime,
      EndTime = e.EndTime,
      Reason = e.Reason
    }).ToList();

    await SendOkAsync(Result.Success(response), ct);
  }
}
