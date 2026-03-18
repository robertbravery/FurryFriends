using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.Enums;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

internal class UpdateTimeslotHandler : ICommandHandler<UpdateTimeslotCommand, Result<TimeslotDto>>
{
    private readonly IRepository<TimeslotEntity> _timeslotRepository;
    private readonly IRepository<PetWalker> _petWalkerRepository;
    private readonly ILogger<UpdateTimeslotHandler> _logger;

    public UpdateTimeslotHandler(
        IRepository<TimeslotEntity> timeslotRepository,
        IRepository<PetWalker> petWalkerRepository,
        ILogger<UpdateTimeslotHandler> logger)
    {
        _timeslotRepository = timeslotRepository;
        _petWalkerRepository = petWalkerRepository;
        _logger = logger;
    }

    public async Task<Result<TimeslotDto>> Handle(UpdateTimeslotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Updating timeslot: {TimeslotId}, StartTime: {StartTime}, Duration: {Duration}",
                request.TimeslotId,
                request.StartTime,
                request.DurationInMinutes);

            // Validate duration (additional validation beyond FluentValidation)
            if (request.DurationInMinutes < 30 || request.DurationInMinutes > 45)
            {
                return Result<TimeslotDto>.Error("DurationInMinutes must be between 30 and 45 minutes.");
            }

            // Get existing timeslot
            var timeslotSpec = new TimeslotByIdSpec(request.TimeslotId);
            var existingTimeslot = await _timeslotRepository.FirstOrDefaultAsync(timeslotSpec, cancellationToken);

            if (existingTimeslot == null)
            {
                _logger.LogWarning("Timeslot not found: {TimeslotId}", request.TimeslotId);
                return Result.NotFound("Timeslot not found");
            }

            // Only Available timeslots can be updated
            if (existingTimeslot.Status != TimeslotStatus.Available)
            {
                return Result<TimeslotDto>.Error("Only Available timeslots can be updated.");
            }

            // Calculate end time
            var endTime = request.StartTime.AddMinutes(request.DurationInMinutes);

            // Check if pet walker has working hours (schedule) for this day
            // Get the PetWalker with their schedules
            var petWalkerSpec = new GetPetWalkerByIdSpecification(existingTimeslot.PetWalkerId);
            var petWalker = await _petWalkerRepository.FirstOrDefaultAsync(petWalkerSpec, cancellationToken);

            if (petWalker == null)
            {
                return Result<TimeslotDto>.Error("Pet walker not found.");
            }

            // Check if pet walker has a schedule for this day
            var schedules = petWalker.Schedules.ToList();
            var daySchedules = schedules.Where(s => s.DayOfWeek == existingTimeslot.Date.DayOfWeek).ToList();

            if (!daySchedules.Any())
            {
                return Result<TimeslotDto>.Error("Pet walker does not have working hours set for this day.");
            }

            // Check if timeslot is within working hours (schedule)
            var isWithinWorkingHours = daySchedules.Any(s =>
                request.StartTime >= s.StartTime &&
                endTime <= s.EndTime);

            if (!isWithinWorkingHours)
            {
                return Result<TimeslotDto>.Error("Timeslot must be within working hours.");
            }

            // Check for overlapping timeslots (excluding the current one)
            var overlappingSpec = new OverlappingTimeslotsSpec(
                existingTimeslot.PetWalkerId,
                existingTimeslot.Date,
                request.StartTime,
                endTime);

            var overlappingTimeslots = await _timeslotRepository.ListAsync(overlappingSpec, cancellationToken);

            // Filter out the current timeslot we're updating
            var otherOverlappingTimeslots = overlappingTimeslots.Where(t => t.Id != request.TimeslotId).ToList();

            if (otherOverlappingTimeslots.Any())
            {
                return Result<TimeslotDto>.Error("Timeslot overlaps with an existing timeslot.");
            }

            // Update the timeslot
            existingTimeslot.UpdateStartTimeAndDuration(request.StartTime, request.DurationInMinutes);

            await _timeslotRepository.UpdateAsync(existingTimeslot, cancellationToken);

            _logger.LogInformation(
                "Updated timeslot {TimeslotId} successfully",
                existingTimeslot.Id);

            var dto = new TimeslotDto(
                existingTimeslot.Id,
                existingTimeslot.PetWalkerId,
                existingTimeslot.Date,
                existingTimeslot.StartTime,
                existingTimeslot.EndTime,
                existingTimeslot.DurationInMinutes,
                existingTimeslot.Status);

            return Result<TimeslotDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating timeslot {TimeslotId}", request.TimeslotId);
            return Result<TimeslotDto>.Error(ex.Message);
        }
    }
}
