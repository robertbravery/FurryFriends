using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;
using Microsoft.Extensions.Logging;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

internal class CreateWorkingHoursHandler : ICommandHandler<CreateWorkingHoursCommand, Result<WorkingHoursDto>>
{
    private readonly IRepository<WorkingHoursEntity> _workingHoursRepository;
    private readonly ILogger<CreateWorkingHoursHandler> _logger;

    public CreateWorkingHoursHandler(IRepository<WorkingHoursEntity> workingHoursRepository, ILogger<CreateWorkingHoursHandler> logger)
    {
        _workingHoursRepository = workingHoursRepository;
        _logger = logger;
    }

    public async Task<Result<WorkingHoursDto>> Handle(CreateWorkingHoursCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate that EndTime > StartTime
            if (request.EndTime <= request.StartTime)
            {
                return Result<WorkingHoursDto>.Error("End time must be after start time");
            }

            // Check for overlapping shifts on the same day
            var existingWorkingHoursSpec = new WorkingHoursByPetWalkerAndDaySpec(request.PetWalkerId, request.DayOfWeek);
            var existingWorkingHours = await _workingHoursRepository.ListAsync(existingWorkingHoursSpec, cancellationToken);

            foreach (var existing in existingWorkingHours)
            {
                // Check if times overlap
                if (request.StartTime < existing.EndTime && request.EndTime > existing.StartTime)
                {
                    return Result<WorkingHoursDto>.Error($"Working hours overlap with existing shift on {request.DayOfWeek}");
                }
            }

            // Create the working hours using the domain entity
            var createResult = WorkingHoursEntity.Create(
                request.PetWalkerId,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.IsActive);

            if (!createResult.IsSuccess)
            {
                return Result<WorkingHoursDto>.Error(new ErrorList(createResult.Errors));
            }

            var workingHours = createResult.Value;
            await _workingHoursRepository.AddAsync(workingHours, cancellationToken);

            _logger.LogInformation("Created working hours for PetWalker {PetWalkerId} on {DayOfWeek}", 
                request.PetWalkerId, request.DayOfWeek);

            var dto = new WorkingHoursDto(
                workingHours.Id,
                workingHours.PetWalkerId,
                workingHours.DayOfWeek,
                workingHours.StartTime,
                workingHours.EndTime,
                workingHours.IsActive);

            return Result<WorkingHoursDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating working hours for PetWalker {PetWalkerId}", request.PetWalkerId);
            return Result<WorkingHoursDto>.Error(ex.Message);
        }
    }
}
