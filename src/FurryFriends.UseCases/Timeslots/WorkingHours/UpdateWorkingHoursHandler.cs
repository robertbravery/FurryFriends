using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;
using Microsoft.Extensions.Logging;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

internal class UpdateWorkingHoursHandler : ICommandHandler<UpdateWorkingHoursCommand, Result<WorkingHoursDto>>
{
    private readonly IRepository<WorkingHoursEntity> _workingHoursRepository;
    private readonly ILogger<UpdateWorkingHoursHandler> _logger;

    public UpdateWorkingHoursHandler(IRepository<WorkingHoursEntity> workingHoursRepository, ILogger<UpdateWorkingHoursHandler> logger)
    {
        _workingHoursRepository = workingHoursRepository;
        _logger = logger;
    }

    public async Task<Result<WorkingHoursDto>> Handle(UpdateWorkingHoursCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate that EndTime > StartTime
            if (request.EndTime <= request.StartTime)
            {
                return Result<WorkingHoursDto>.Error("End time must be after start time");
            }

            // Get existing working hours
            var spec = new WorkingHoursByIdSpec(request.Id);
            var workingHours = await _workingHoursRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (workingHours == null)
            {
                return Result<WorkingHoursDto>.NotFound("Working hours not found");
            }

            // Check for overlapping shifts on the same day (excluding current record)
            var existingWorkingHoursSpec = new WorkingHoursByPetWalkerAndDaySpec(workingHours.PetWalkerId, workingHours.DayOfWeek);
            var existingWorkingHours = await _workingHoursRepository.ListAsync(existingWorkingHoursSpec, cancellationToken);

            foreach (var existing in existingWorkingHours.Where(w => w.Id != request.Id))
            {
                // Check if times overlap
                if (request.StartTime < existing.EndTime && request.EndTime > existing.StartTime)
                {
                    return Result<WorkingHoursDto>.Error($"Working hours overlap with existing shift on {workingHours.DayOfWeek}");
                }
            }

            // Update the working hours
            // Using reflection to set the private setters or creating a new entity
            // Since the entity uses private setters, we need to update via reflection or create new
            var updatedWorkingHours = WorkingHoursEntity.Create(
                workingHours.PetWalkerId,
                workingHours.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.IsActive);

            if (!updatedWorkingHours.IsSuccess)
            {
                return Result<WorkingHoursDto>.Error(new ErrorList(updatedWorkingHours.Errors));
            }

            // We need to delete the old one and add the new one since it's a value object pattern
            await _workingHoursRepository.DeleteAsync(workingHours, cancellationToken);
            await _workingHoursRepository.AddAsync(updatedWorkingHours.Value, cancellationToken);

            _logger.LogInformation("Updated working hours {Id}", request.Id);

            var dto = new WorkingHoursDto(
                updatedWorkingHours.Value.Id,
                updatedWorkingHours.Value.PetWalkerId,
                updatedWorkingHours.Value.DayOfWeek,
                updatedWorkingHours.Value.StartTime,
                updatedWorkingHours.Value.EndTime,
                updatedWorkingHours.Value.IsActive);

            return Result<WorkingHoursDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating working hours {Id}", request.Id);
            return Result<WorkingHoursDto>.Error(ex.Message);
        }
    }
}
