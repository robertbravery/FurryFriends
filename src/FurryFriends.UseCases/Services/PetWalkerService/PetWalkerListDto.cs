using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCases.Services.PetWalkerService;

public record PetWalkerListDto(List<PetWalker> Users, int TotalCount);
