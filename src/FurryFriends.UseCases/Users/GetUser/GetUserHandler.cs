using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.UserAggregate.Enums;
using FurryFriends.UseCases.Services;

namespace FurryFriends.UseCases.Users.GetUser;
public class GetUserHandler( IUserService _userService) : IQueryHandler<GetUserQuery, Result<UserDto>>
{
  public async Task<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
  {
    var entityResult = await _userService.GetUserByEmailAsync(query.Email, cancellationToken);
    if(!entityResult.IsSuccess) return Result.NotFound("User Not Found with the given email"); ;
    var entity = entityResult.Value;
    var bioPicture = GetBioPicture(entity);
    var photos = GetPhotoList(entity);
    return new UserDto(entity.Id, entity.Email, entity.Name.FullName, entity.PhoneNumber.ToString(), entity.Address.City, bioPicture, photos);
  }

  private static List<PhotoDto>? GetPhotoList(User entity)
  {
    var photos = entity.Photos.Where(x => x.PhotoType == PhotoType.PetWalkerPhoto)
      .Select(x => new PhotoDto(x.Url, x.Description)).ToList();
    return photos;
  }
  private static PhotoDto? GetBioPicture(User entity)
  {    
    var photo = entity.Photos.FirstOrDefault(x => x.PhotoType == PhotoType.BioPic);
    if (photo == null) return null;

    return new PhotoDto(photo.Url, photo.Description);
  }
}
