namespace FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

public record DeletePetWalkerDateExceptionCommand(Guid PetWalkerId, DateTime Date) : ICommand<Result>;
