using FurryFriends.Core.Entities;
using FurryFriends.UseCases.Contributors;

namespace FurryFriends.UseCases.Users.List;
public record ListUsersQuery(string? SearchString,  int PageNumber=1, int PageSize = 10) : IQuery<Result<(List<User> Users, int TotalCount)>>;
