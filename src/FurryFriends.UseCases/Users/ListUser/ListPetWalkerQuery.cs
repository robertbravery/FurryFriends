using FurryFriends.UseCase.Services.DataTransferObjects;

namespace FurryFriends.UseCase.Users.ListUser;
public record ListPetWalkerQuery(string? SearchString, int PageNumber = 1, int PageSize = 10) : IQuery<Result<PetWalkerListDto>>;
