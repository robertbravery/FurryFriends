using FurryFriends.UseCases.Services.DataTransferObjects;

namespace FurryFriends.UseCases.PetWalkers.ListPetWalker;
public record ListPetWalkerQuery(string? SearchString, int PageNumber = 1, int PageSize = 10) : IQuery<Result<PetWalkerListDto>>;
