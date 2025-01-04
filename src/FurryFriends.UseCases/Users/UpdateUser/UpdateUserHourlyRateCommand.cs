namespace FurryFriends.UseCases.Users.UpdateUser;

public record UpdateUserHourlyRateCommand(Guid UserId, decimal HourlyRate, string Currency) : ICommand<Result<bool>>;
