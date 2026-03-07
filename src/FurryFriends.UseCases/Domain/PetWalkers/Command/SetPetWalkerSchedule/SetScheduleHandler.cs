using Ardalis.GuardClauses;
//using FastEndpoints;
using FurryFriends.Core.PetWalkerAggregate;
using Mediator;

//using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.SetPetWalkerSchedule;

public class SetScheduleHandler :  ICommandHandler<SetScheduleCommand>
{
  private readonly IRepository<PetWalker> _walkerRepo;

  public SetScheduleHandler(IRepository<PetWalker> walkerRepo)
  {
    _walkerRepo = walkerRepo;
  }

  public ValueTask<Unit> Handle(SetScheduleCommand command, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  //async ValueTask<Unit> ICommandHandler<SetScheduleCommand, Unit>.Handle(SetScheduleCommand command, CancellationToken cancellationToken)
  //{
  //  try
  //  {
  //    var walker = await _walkerRepo.GetByIdAsync(command.PetWalkerId, cancellationToken)
  //        ?? throw new NotFoundException("PetWalker ID", command.PetWalkerId.ToString());

  //    walker.ClearSchedules();

  //    foreach (var dto in command.Schedules)
  //    {
  //      walker.AddSchedule(new Schedule(dto.DayOfWeek, dto.StartTime, dto.EndTime));
  //    }
  //    await _walkerRepo.UpdateAsync(walker, cancellationToken);
  //  }
  //  catch (Exception ex)
  //  {

  //    throw new Exception(ex.Message);
  //  }

  //  return Unit.Value;
  //}
}

