namespace FurryFriends.UseCase.Users.GetUser;

public record GetUserQuery(string Email) : IQuery<Result<UserDto>>;
