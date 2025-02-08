namespace FurryFriends.UseCases.PetWalkers.GetPetWalker;

public record GetPetWalkerQuery(string EmailAddress) : IQuery<Result<PetWalkerDto>>;
