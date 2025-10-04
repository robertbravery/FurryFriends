using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Enums;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using FurryFriends.UseCases.Services.PictureService;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetPicture;

public class GetPetWalkerPhotoHandler : IQueryHandler<GetPetWalkerPhotoQuery, Result<DetailPictureDto>>
{
  private readonly IPictureService _pictureService;

  public GetPetWalkerPhotoHandler(IPictureService pictureService)
  {
    _pictureService = pictureService;
  }

  public async Task<Result<DetailPictureDto>> Handle(GetPetWalkerPhotoQuery query, CancellationToken cancellationToken)
  {
    var entityResult = await _pictureService.GetPetWalkerPictureAsync(query.PetWalkerId, cancellationToken);
    if (!entityResult.IsSuccess)
    {
      return Result.NotFound("User Not Found with the given email");
    }
    var petwalkerPhoto = DetailPictureDto.Map(entityResult.Value);
    return petwalkerPhoto;
  }

  private static List<PhotoDto>? GetPhotoList(PetWalker entity)
  {
    var photos = entity.Photos.Where(x => x.PhotoType == PhotoType.PetWalkerPhoto)
        .Select(x => new PhotoDto(x.Url, x.Description)).ToList();
    return photos;
  }

  private static PhotoDto? GetBioPicture(PetWalker entity)
  {
    var photo = entity.Photos.FirstOrDefault(x => x.PhotoType == PhotoType.BioPic);
    if (photo == null) return null;

    return new PhotoDto(photo.Url, photo.Description);
  }
}

