using Ardalis.GuardClauses;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

/// <summary>
/// Handler for getting PetWalker schedule
/// </summary>
public class GetPetWalkerScheduleHandler : IRequestHandler<GetPetWalkerScheduleQuery, Result<List<ScheduleDto>>>
{
  private readonly IRepository<PetWalker> _walkerRepo;

  public GetPetWalkerScheduleHandler(IRepository<PetWalker> walkerRepo)
  {
    _walkerRepo = walkerRepo;
  }

  public async Task<Result<List<ScheduleDto>>> Handle(GetPetWalkerScheduleQuery request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request, nameof(request));
    Guard.Against.Default(request.PetWalkerId, nameof(request.PetWalkerId));

    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken);
    
    if (walker == null)
    {
      return Result.NotFound($"PetWalker with ID {request.PetWalkerId} not found");
    }

    var schedules = walker.Schedules
      .Select(s => new ScheduleDto(s.DayOfWeek, s.StartTime, s.EndTime))
      .ToList();

    return Result.Success(schedules);
  }
}
