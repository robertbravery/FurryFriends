namespace FurryFriends.UseCases.Users.UpdateUser;

public record UpdatePetWalkerHourlyRateCommand(Guid UserId, decimal HourlyRate, string Currency) : ICommand<Result<bool>>;
