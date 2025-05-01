namespace FurryFriends.BlazorUI.Client.Models.Picture;

public class PictureViewModel
{
  public Guid PetWalkerId { get; set; }
  public string PetwalkerName { get; set; } = default!;
  public DetailedPhotoDto? ProfilePicture { get; set; }
  public List<DetailedPhotoDto> Photos { get; set; } = new List<DetailedPhotoDto>();
}
