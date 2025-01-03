using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Users.ListUser;

public record ListUsersByLocationQuery(string? SearchString, Guid? Location, int PageNumber = 1, int PageSize = 10) : IQuery<Result<(List<User> Users, int TotalCount)>>;
