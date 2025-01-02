using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Users.ListUser;
public record ListUsersQuery(string? SearchString, int PageNumber = 1, int PageSize = 10) : IQuery<Result<(List<User> Users, int TotalCount)>>;
