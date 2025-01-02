namespace FurryFriends.UseCases.Users.GetUser;

public record UserDto(Guid Id, string Email, string Name, string PhoneNumber, string Address);
