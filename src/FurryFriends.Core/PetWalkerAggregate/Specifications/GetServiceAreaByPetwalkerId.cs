namespace FurryFriends.Core.PetWalkerAggregate.Specifications;

public class GetServiceAreaByPetwalkerId : Specification<ServiceArea>
{
  public GetServiceAreaByPetwalkerId(Guid petwalkerId) =>
    Query
        .Where(w => w.UserID == petwalkerId);
}

