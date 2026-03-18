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

internal class CreateTimeslotHandler : ICommandHandler<CreateTimeslotCommand, Result<TimeslotDto>>
{
    private readonly IRepository<TimeslotEntity> _timeslotRepository;
    private readonly IRepository<PetWalker> _petWalkerRepository;
    private readonly ILogger<CreateTimeslotHandler> _logger;

    public CreateTimeslotHandler(
        IRepository<TimeslotEntity> timeslotRepository,
        IRepository<PetWalker> petWalkerRepository,
        ILogger<CreateTimeslotHandler> logger)
    {
        _timeslotRepository = timeslotRepository;
        _petWalkerRepository = petWalkerRepository;
        _logger = logger;
    }

    public async Task<Result<TimeslotDto>> Handle(CreateTimeslotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate duration (additional validation beyond FluentValidation)
            if (request.DurationInMinutes < 30 || request.DurationInMinutes > 45)
            {
                return Result<TimeslotDto>.Error("DurationInMinutes must be between 30 and 45 minutes.");
            }

            // Calculate end time
            var endTime = request.StartTime.AddMinutes(request.DurationInMinutes);

            // Check if pet walker has working hours (schedule) for this day
            // Get the PetWalker with their schedules
            var petWalkerSpec = new GetPetWalkerByIdSpecification(request.PetWalkerId);
            var petWalker = await _petWalkerRepository.FirstOrDefaultAsync(petWalkerSpec, cancellationToken);

            if (petWalker == null)
            {
                return Result<TimeslotDto>.Error("Pet walker not found.");
            }

            // Check if pet walker has a schedule for this day
            var schedules = petWalker.Schedules.ToList();
            var daySchedules = schedules.Where(s => s.DayOfWeek == request.Date.DayOfWeek).ToList();

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

            // Check for overlapping timeslots
            var overlappingSpec = new OverlappingTimeslotsSpec(
                request.PetWalkerId,
                request.Date,
                request.StartTime,
                endTime);

            var overlappingTimeslots = await _timeslotRepository.ListAsync(overlappingSpec, cancellationToken);

            if (overlappingTimeslots != null && overlappingTimeslots.Any())
            {
                return Result<TimeslotDto>.Error("Timeslot overlaps with an existing timeslot.");
            }

            // Create the timeslot using the domain entity
            var createResult = TimeslotEntity.Create(
                request.PetWalkerId,
                request.Date,
                request.StartTime,
                request.DurationInMinutes,
                TimeslotStatus.Available);

            if (!createResult.IsSuccess)
            {
                return Result<TimeslotDto>.Error(new ErrorList(createResult.Errors));
            }

            var timeslot = createResult.Value;
            await _timeslotRepository.AddAsync(timeslot, cancellationToken);

            _logger.LogInformation(
                "Created timeslot {TimeslotId} for PetWalker {PetWalkerId} on {Date} at {StartTime}",
                timeslot.Id, request.PetWalkerId, request.Date, request.StartTime);

            var dto = new TimeslotDto(
                timeslot.Id,
                timeslot.PetWalkerId,
                timeslot.Date,
                timeslot.StartTime,
                timeslot.EndTime,
                timeslot.DurationInMinutes,
                timeslot.Status);

            return Result<TimeslotDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating timeslot for PetWalker {PetWalkerId}", request.PetWalkerId);
            return Result<TimeslotDto>.Error(ex.Message);
        }
    }
}
