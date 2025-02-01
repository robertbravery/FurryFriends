namespace FurryFriends.UseCases.PetWalkers.UpdatePetWalker;

public record UpdatePetWalkerHourlyRateCommand(Guid UserId, decimal HourlyRate, string Currency) : ICommand<Result<bool>>;
