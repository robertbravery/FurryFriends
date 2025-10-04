using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Enums;
using Microsoft.AspNetCore.Http;

namespace FurryFriends.UseCases.Services.PictureService;

public interface IPictureService
{
  Task<Result<Photo>> UpdatePetWalkerBioPictureAsync(Guid petWalkerId, IFormFile file, CancellationToken cancellationToken);
  Task<Result<PetWalker>> GetPetWalkerPictureAsync(Guid petWalkerId, CancellationToken cancellationToken);
  Task<Result<Photo>> UpdatePetWalkerPhotoAsync(Guid petWalkerId, Guid photoId, IFormFile file, string? description, CancellationToken cancellationToken);
  Task<Result<Photo>> AddPetWalkerPhotoAsync(Guid petWalkerId, IFormFile file, string? description, PhotoType photoType, CancellationToken cancellationToken);
}
