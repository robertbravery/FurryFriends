using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetAvailability;

public class GetPetWalkerAvailabilitySettingsHandler
    : IQueryHandler<GetPetWalkerAvailabilitySettingsQuery, Result<PetWalkerAvailabilitySettingsDto>>
{
  private readonly IRepository<PetWalker> _walkerRepo;

  public GetPetWalkerAvailabilitySettingsHandler(IRepository<PetWalker> walkerRepo)
  {
    _walkerRepo = walkerRepo;
  }

  public async Task<Result<PetWalkerAvailabilitySettingsDto>> Handle(
      GetPetWalkerAvailabilitySettingsQuery request,
      CancellationToken cancellationToken)
  {
    if (request.PetWalkerId == Guid.Empty)
    {
      return Result<PetWalkerAvailabilitySettingsDto>.Error("PetWalker ID is required");
    }

    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken);
    if (walker == null)
    {
      return Result<PetWalkerAvailabilitySettingsDto>.NotFound($"PetWalker {request.PetWalkerId} not found");
    }

    return Result<PetWalkerAvailabilitySettingsDto>.Success(new PetWalkerAvailabilitySettingsDto(
        walker.Id,
        walker.SlotDurationMinutes,
        walker.InterBookingGapMinutes,
        walker.DailyPetWalkLimit));
  }
}
