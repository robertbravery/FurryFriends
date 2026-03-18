using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

internal class DeleteTimeslotHandler : ICommandHandler<DeleteTimeslotCommand, Result<bool>>
{
    private readonly IRepository<TimeslotEntity> _timeslotRepository;
    private readonly ILogger<DeleteTimeslotHandler> _logger;

    public DeleteTimeslotHandler(
        IRepository<TimeslotEntity> timeslotRepository,
        ILogger<DeleteTimeslotHandler> logger)
    {
        _timeslotRepository = timeslotRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteTimeslotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Deleting timeslot: {TimeslotId}",
                request.TimeslotId);

            // Get existing timeslot
            var timeslotSpec = new TimeslotByIdSpec(request.TimeslotId);
            var existingTimeslot = await _timeslotRepository.FirstOrDefaultAsync(timeslotSpec, cancellationToken);

            if (existingTimeslot == null)
            {
                _logger.LogWarning("Timeslot not found: {TimeslotId}", request.TimeslotId);
                return Result.NotFound("Timeslot not found");
            }

            // Only Available or Cancelled timeslots can be deleted
            if (existingTimeslot.Status != TimeslotStatus.Available && 
                existingTimeslot.Status != TimeslotStatus.Cancelled)
            {
                return Result<bool>.Error("Only Available or Cancelled timeslots can be deleted.");
            }

            await _timeslotRepository.DeleteAsync(existingTimeslot, cancellationToken);

            _logger.LogInformation(
                "Deleted timeslot {TimeslotId} successfully",
                existingTimeslot.Id);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting timeslot {TimeslotId}", request.TimeslotId);
            return Result<bool>.Error(ex.Message);
        }
    }
}
