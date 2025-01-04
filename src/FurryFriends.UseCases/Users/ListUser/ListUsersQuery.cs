using FurryFriends.Core.UserAggregate;
using FurryFriends.UseCases.Services.DataTransferObjects;

namespace FurryFriends.UseCases.Users.ListUser;
public record ListUsersQuery(string? SearchString, int PageNumber = 1, int PageSize = 10) : IQuery<Result<UserListDto>>;
