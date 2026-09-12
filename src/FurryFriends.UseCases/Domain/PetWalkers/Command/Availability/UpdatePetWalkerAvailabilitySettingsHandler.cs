using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

public class UpdatePetWalkerAvailabilitySettingsHandler
    : ICommandHandler<UpdatePetWalkerAvailabilitySettingsCommand, Result<PetWalkerAvailabilitySettingsDto>>
{
  private readonly IRepository<PetWalker> _walkerRepo;
  private readonly ILogger<UpdatePetWalkerAvailabilitySettingsHandler> _logger;

  public UpdatePetWalkerAvailabilitySettingsHandler(
      IRepository<PetWalker> walkerRepo,
      ILogger<UpdatePetWalkerAvailabilitySettingsHandler> logger)
  {
    _walkerRepo = walkerRepo;
    _logger = logger;
  }

  public async Task<Result<PetWalkerAvailabilitySettingsDto>> Handle(
      UpdatePetWalkerAvailabilitySettingsCommand request,
      CancellationToken cancellationToken)
  {
    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken);
    if (walker == null)
    {
      return Result<PetWalkerAvailabilitySettingsDto>.NotFound($"PetWalker {request.PetWalkerId} not found");
    }

    try
    {
      walker.UpdateSlotDurationMinutes(request.SlotDurationMinutes);
      walker.UpdateInterBookingGapMinutes(request.InterBookingGapMinutes);
      walker.UpdateDailyPetWalkLimit(request.DailyPetWalkLimit);
    }
    catch (Exception ex)
    {
      // Guards on the aggregate reject out-of-range values; surface them as validation
      // errors rather than a 500.
      _logger.LogWarning(ex, "Rejected availability settings for PetWalker {PetWalkerId}", request.PetWalkerId);
      return Result<PetWalkerAvailabilitySettingsDto>.Error(ex.Message);
    }

    await _walkerRepo.UpdateAsync(walker, cancellationToken);

    _logger.LogInformation(
        "Updated availability settings for PetWalker {PetWalkerId}: slot={SlotMinutes}min, gap={GapMinutes}min, dailyLimit={DailyLimit}",
        walker.Id, walker.SlotDurationMinutes, walker.InterBookingGapMinutes, walker.DailyPetWalkLimit);

    return Result<PetWalkerAvailabilitySettingsDto>.Success(new PetWalkerAvailabilitySettingsDto(
        walker.Id,
        walker.SlotDurationMinutes,
        walker.InterBookingGapMinutes,
        walker.DailyPetWalkLimit));
  }
}
