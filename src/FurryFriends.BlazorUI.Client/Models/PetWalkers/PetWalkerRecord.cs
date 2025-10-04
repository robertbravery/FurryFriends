using FurryFriends.BlazorUI.Client.Models.Picture;

namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

public class PetWalkerRecord
{
  public Guid Id { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public IEnumerable<string>? Locations { get; set; }
  public DetailedPhotoDto? BioPicture { get; set; }
  public IEnumerable<DetailedPhotoDto>? Photos { get; set; }
}
