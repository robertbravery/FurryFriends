using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.Core.PetWalkerAggregate.Specifications;

/// <summary>
/// Loads the date exception (if any) for a PetWalker on a specific date.
/// At most one can exist - enforced by a unique index on (PetWalkerId, Date).
/// </summary>
public sealed class PetWalkerDateExceptionByPetWalkerAndDateSpec : Specification<PetWalkerDateException>
{
  public PetWalkerDateExceptionByPetWalkerAndDateSpec(Guid petWalkerId, DateOnly date)
  {
    Query
        .Where(e => e.PetWalkerId == petWalkerId)
        .Where(e => e.Date == date);
  }
}

/// <summary>
/// Lists every date exception for a PetWalker, ordered for display.
/// </summary>
public sealed class PetWalkerDateExceptionsByPetWalkerSpec : Specification<PetWalkerDateException>
{
  public PetWalkerDateExceptionsByPetWalkerSpec(Guid petWalkerId)
  {
    Query
        .Where(e => e.PetWalkerId == petWalkerId)
        .OrderBy(e => e.Date);
  }
}
