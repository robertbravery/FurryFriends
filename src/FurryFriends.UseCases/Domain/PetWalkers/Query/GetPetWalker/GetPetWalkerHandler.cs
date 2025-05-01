using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Enums;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using FurryFriends.UseCases.Services.PetWalkerService;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetPetWalker;
public class GetPetWalkerHandler(IPetWalkerService _userService) : IQueryHandler<GetPetWalkerQuery, Result<PetWalkerDto>>
{
  public async Task<Result<PetWalkerDto>> Handle(GetPetWalkerQuery query, CancellationToken cancellationToken)
  {
    var entityResult = await _userService.GetPetWalkerByEmailAsync(query.EmailAddress, cancellationToken);
    if (!entityResult.IsSuccess)
    {
      return Result.NotFound("User Not Found with the given email");
    }
    var entity = entityResult.Value;
    var bioPicture = GetBioPicture(entity);
    var photos = GetPhotoList(entity);
    return new PetWalkerDto(
        entity.Id,
        entity.Email.EmailAddress,
        entity.Name.FullName,
        entity.PhoneNumber.CountryCode,
        entity.PhoneNumber.Number,
        entity.Address.Street,
        entity.Address.City,
        entity.Address.StateProvinceRegion,
        entity.Address.ZipCode,
        entity.Address.Country,
        entity.Biography,
        entity.DateOfBirth,
        entity.Gender?.Gender.ToString() ?? "N/A",
        entity.Compensation.HourlyRate,
        entity.Compensation.Currency,
        entity.IsActive,
        entity.IsVerified,
        entity.YearsOfExperience,
        entity.HasInsurance,
        entity.HasFirstAidCertificatation,
        entity.DailyPetWalkLimit,
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

