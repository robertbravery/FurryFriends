namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;

public record UpdatePetWalkerHourlyRateCommand(Guid UserId, decimal HourlyRate, string Currency) : ICommand<Result<bool>>;
