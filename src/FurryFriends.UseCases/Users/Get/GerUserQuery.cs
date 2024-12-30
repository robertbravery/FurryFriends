using FurryFriends.UseCases.Users.Get;

public record GetUserQuery(string Email) : IQuery<Result<UserDto>>;
