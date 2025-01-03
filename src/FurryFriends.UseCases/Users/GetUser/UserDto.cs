namespace FurryFriends.UseCases.Users.GetUser;

public record UserDto(
  Guid Id,
  string Email,
  string Name,
  string PhoneNumber,
  string Address,
  List<string>? ServiceLocation,
  PhotoDto? BioPicture, 
  List<PhotoDto>? Photos);
