namespace FurryFriends.UseCases.PetWalkers.GetPetWalker;

public record GetPetWalkerQuery(string Email) : IQuery<Result<PetWalkerDto>>;
