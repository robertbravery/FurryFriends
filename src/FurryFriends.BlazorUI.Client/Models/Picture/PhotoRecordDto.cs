using FurryFriends.BlazorUI.Client.Models.Picture.Enums;

namespace FurryFriends.BlazorUI.Client.Models.Picture;

public class DetailedPhotoDto
{
  public Guid Id { get; set; }
  public PhotoType PhotoType { get; set; } = PhotoType.BioPic;
  public string Url { get; set; } = string.Empty;
  public string? Description { get; set; }
}
