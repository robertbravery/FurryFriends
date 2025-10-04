namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Schedule;

public class SetScheduleValidator : AbstractValidator<SetScheduleCommand>
{
  public SetScheduleValidator()
  {
    RuleForEach(x => x.Schedules).ChildRules(schedule =>
    {
      schedule.RuleFor(s => s.StartTime)
              .LessThan(s => s.EndTime)
              .WithMessage("Start time must be before end time");
    });
  }
}
