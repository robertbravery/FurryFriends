namespace FurryFriends.UseCases.Services.PetWalkerService;

public interface IServiceAreaService
{
  Task<Result> AddServiceAreaAsync(Guid petWalkerId, Guid localityId, CancellationToken cancellationToken = default);
  Task<Result> RemoveServiceAreaAsync(Guid serviceAreaId, CancellationToken cancellationToken = default);

}
