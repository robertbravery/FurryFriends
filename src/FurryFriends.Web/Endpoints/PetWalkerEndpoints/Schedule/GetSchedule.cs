using Ardalis.GuardClauses;
using FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Schedule;

/// <summary>
/// Endpoint for getting PetWalker schedules
/// </summary>
public class GetSchedule(IMediator Mediator) : Endpoint<GetScheduleRequest, Result<GetScheduleResponse>>
{
  private readonly IMediator _mediator = Mediator;

  public override void Configure()
  {
    Get(GetScheduleRequest.Route);
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("GetPetWalkerSchedule_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<GetScheduleResponse>(200)
        .Produces(400)
        .Produces(404));
  }

  public override async Task HandleAsync(GetScheduleRequest request, CancellationToken ct)
  {
    Guard.Against.Null(request, nameof(GetScheduleRequest));
    Guard.Against.Default(request.PetWalkerId, nameof(request.PetWalkerId));

    var query = new GetPetWalkerScheduleQuery(request.PetWalkerId);
    var result = await _mediator.Send(query, ct);

    if (result.IsSuccess)
    {
      var response = new GetScheduleResponse
      {
        PetWalkerId = request.PetWalkerId,
        Schedules = result.Value.Select(s => new ScheduleItemResponse
        {
          DayOfWeek = s.DayOfWeek,
          StartTime = s.StartTime,
          EndTime = s.EndTime
        }).ToList()
      };

      await SendOkAsync(Result.Success(response), ct);
    }
    else
    {
      //await SendResultAsync(result.ToResult<GetScheduleResponse>(), ct);
      await SendNotFoundAsync(ct);
    }
  }
}
