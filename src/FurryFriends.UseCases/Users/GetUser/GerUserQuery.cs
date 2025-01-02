namespace FurryFriends.UseCases.Users.GetUser;

public record GetUserQuery(string Email) : IQuery<Result<UserDto>>;
