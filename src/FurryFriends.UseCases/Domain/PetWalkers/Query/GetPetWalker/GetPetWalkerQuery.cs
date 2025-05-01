using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetPetWalker;

public record GetPetWalkerQuery(string EmailAddress) : IQuery<Result<PetWalkerDto>>;
