namespace FurryFriends.UseCases.Users.Get;

public record UserDto(Guid Id, string Email, string Name, string PhoneNumber, string Address);
