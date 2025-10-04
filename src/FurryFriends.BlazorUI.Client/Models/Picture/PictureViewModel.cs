namespace FurryFriends.BlazorUI.Client.Models.Picture;

public class PictureViewModel
{
  public Guid PetWalkerId { get; set; }
  public string PetWalkerName { get; set; } = default!;
  public DetailedPhotoDto? ProfilePicture { get; set; }
  public List<DetailedPhotoDto> Photos { get; set; } = new List<DetailedPhotoDto>();
}
