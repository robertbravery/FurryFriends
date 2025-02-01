using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCase.Services.DataTransferObjects;

public record PetWalkerListDto(List<PetWalker> Users, int TotalCount);
