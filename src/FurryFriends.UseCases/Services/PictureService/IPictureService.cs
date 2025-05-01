using FurryFriends.Core.PetWalkerAggregate;
using Microsoft.AspNetCore.Http;

namespace FurryFriends.UseCases.Services.PictureService;

public interface IPictureService
{
  Task UpdatePetWalkerBioPictureAsync(Guid petWalkerId, IFormFile file, CancellationToken cancellationToken);
  Task<Result<PetWalker>> GetPetWalkerPictureAsync(Guid petWalkerId, CancellationToken cancellationToken);
}
