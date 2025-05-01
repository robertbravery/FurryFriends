using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Enums;

namespace FurryFriends.UseCases.Services.PictureService;

public class DetailPictureDto
{
  public Guid PetWalkerId { get; set; }
  public string PetwalkerName { get; set; } = default!;
  public DetailedPhotoDto? ProfilePicture { get; set; }
  public List<DetailedPhotoDto> Photos { get; set; } = new List<DetailedPhotoDto>();

  internal static DetailPictureDto Map(PetWalker value)
  {
    var result = new DetailPictureDto();
    var profilePicture = value.Photos.FirstOrDefault(x => x.PhotoType == PhotoType.BioPic);
    result.PetWalkerId = value.Id;
    result.PetwalkerName = value.Name.FullName;
    result.ProfilePicture = profilePicture == null ? null : new DetailedPhotoDto { Id = profilePicture!.Id, Url = profilePicture.Url };
    result.Photos = value.Photos.Where(x => x.PhotoType == PhotoType.PetWalkerPhoto).Select(x => new DetailedPhotoDto { Id = x.Id, Url = x.Url, PhotoType = x.PhotoType }).ToList();
    return result;
  }
}
