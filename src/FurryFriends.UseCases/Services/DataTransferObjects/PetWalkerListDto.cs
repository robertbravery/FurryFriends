using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCases.Services.DataTransferObjects;

public record PetWalkerListDto(List<PetWalker> Users, int TotalCount);
