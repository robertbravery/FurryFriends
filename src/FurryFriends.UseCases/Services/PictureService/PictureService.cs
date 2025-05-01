using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using Microsoft.AspNetCore.Http;

namespace FurryFriends.UseCases.Services.PictureService;

public class PictureService(IReadRepository<PetWalker> repository) : IPictureService
{

  public Task UpdatePetWalkerBioPictureAsync(Guid petWalkerId, IFormFile file, CancellationToken cancellationToken)
  {
    // Implementation for updating the bio picture
    // This could involve saving the file to a storage service and updating the database record
    throw new NotImplementedException();
  }

  public async Task<Result<PetWalker>> GetPetWalkerPictureAsync(Guid petWalkerId, CancellationToken cancellationToken)
  {
    var spec = new GetPetWalkerPicturesSpecification(petWalkerId, true);
    var petwalker = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (petwalker is null)
    {
      return Result.NotFound();
    }
    return petwalker;
  }
}
