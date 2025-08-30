using Ardalis.GuardClauses;
using FurryFriends.Core.PetWalkerAggregate;
using MediatR;

public class SetScheduleHandler : IRequestHandler<SetScheduleCommand, Unit>
{
  private readonly IRepository<PetWalker> _walkerRepo;

  public SetScheduleHandler(IRepository<PetWalker> walkerRepo)
  {
    _walkerRepo = walkerRepo;
  }

  public async Task<Unit> Handle(SetScheduleCommand request, CancellationToken cancellationToken)
  {
    try
    {
      var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken)
          ?? throw new NotFoundException("PetWalker ID", request.PetWalkerId.ToString());

      walker.ClearSchedules();

      foreach (var dto in request.Schedules)
      {
        walker.AddSchedule(new Schedule(dto.DayOfWeek, dto.StartTime, dto.EndTime));
      }
      await _walkerRepo.UpdateAsync(walker, cancellationToken);
    }
    catch (Exception ex)
    {

      throw new Exception(ex.Message);
    }

    return Unit.Value;
  }
}

