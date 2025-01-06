using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Services.DataTransferObjects;

public record UserListDto(List<PetWalker> Users, int TotalCount);
