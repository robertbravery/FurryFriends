namespace FurryFriends.UseCases.PetWalkers.GetPetWalker;

public record PetWalkerDto(
  Guid Id,
  string Email,
  string Name,
  string PhoneNumber,
  string Address,
  List<string>? ServiceLocation,
  PhotoDto? BioPicture,
  List<PhotoDto>? Photos);
