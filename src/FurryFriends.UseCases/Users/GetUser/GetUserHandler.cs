using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Enums;
using FurryFriends.UseCases.Services;

namespace FurryFriends.UseCases.Users.GetUser;
public class GetUserHandler(IPetWalkerService _userService) : IQueryHandler<GetUserQuery, Result<UserDto>>
{
  public async Task<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
  {
    var entityResult = await _userService.GetPetWalkerByEmailAsync(query.Email, cancellationToken);
    if (!entityResult.IsSuccess)
    {
      return Result.NotFound("User Not Found with the given email");
    }
    var entity = entityResult.Value;
    var bioPicture = GetBioPicture(entity);
    var photos = GetPhotoList(entity);
    return new UserDto(
      entity.Id,
      entity.Email.EmailAddress,
      entity.Name.FullName,
      entity.PhoneNumber.ToString(),
      entity.Address.City,
      entity.ServiceAreas.Select(x => x.Locality.LocalityName)?.ToList(),
      bioPicture,
      photos);
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
