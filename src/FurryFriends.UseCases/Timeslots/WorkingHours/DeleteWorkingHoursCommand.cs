using Ardalis.Result;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

public record DeleteWorkingHoursCommand(Guid Id) : ICommand<Result<bool>>;
