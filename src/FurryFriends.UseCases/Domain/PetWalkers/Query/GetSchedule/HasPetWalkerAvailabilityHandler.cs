using FurryFriends.Core.PetWalkerAggregate;
using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

/// <summary>
/// Reports whether the walker exists and has at least one weekly schedule configured.
/// A missing walker reports false rather than throwing, so callers can decide the
/// appropriate HTTP result.
/// </summary>
public class HasPetWalkerAvailabilityHandler : IRequestHandler<HasPetWalkerAvailabilityQuery, bool>
{
  private readonly IRepository<PetWalker> _walkerRepo;

  public HasPetWalkerAvailabilityHandler(IRepository<PetWalker> walkerRepo)
  {
    _walkerRepo = walkerRepo;
  }

  public async Task<bool> Handle(HasPetWalkerAvailabilityQuery request, CancellationToken cancellationToken)
  {
    if (request.PetWalkerId == Guid.Empty)
    {
      return false;
    }

    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken);

    return walker?.HasAnySchedule ?? false;
  }
}
