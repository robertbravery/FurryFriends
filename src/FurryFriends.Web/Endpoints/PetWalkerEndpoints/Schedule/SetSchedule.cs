using Ardalis.GuardClauses;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Schedule;

public class SetSchedule(IMediator Mediator) : Endpoint<AddScheduleRequest, Result<SetScheduleCommandBody>>
{
  private readonly IMediator _mediator = Mediator;

  public override void Configure()
  {
    Post(AddScheduleRequest.Route);
    //AllowFileUploads(); // Important for file uploads
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("AddPetWalkerSchedule_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<SetScheduleCommandBody>(201)
        .Produces(400)
        .Produces(404));
  }

  public override async Task HandleAsync(AddScheduleRequest request, CancellationToken ct)
  {
    Guard.Against.Null(request, nameof(AddScheduleRequest));
    Guard.Against.Null(request.PetWalkerId, nameof(request.PetWalkerId));
    Guard.Against.Null(request.Schedules, nameof(request.Schedules));

    var command = CreateCommand(request);
    await _mediator.Send(command, ct);

    // Send success response
    await SendOkAsync(Result.Success(), ct);
  }

  private SetScheduleCommand CreateCommand(AddScheduleRequest request)
  {
    var command = new SetScheduleCommand(
      request.PetWalkerId,
      request.Schedules.Select(s => new ScheduleDto(s.DayOfWeek, s.StartTime, s.EndTime)).ToList()
    );
    return command;
  }
}

