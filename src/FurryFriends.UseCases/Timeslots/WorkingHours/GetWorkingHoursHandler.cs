using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;
using Microsoft.Extensions.Logging;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

internal class GetWorkingHoursHandler : IQueryHandler<GetWorkingHoursQuery, Result<List<WorkingHoursDto>>>
{
    private readonly IRepository<WorkingHoursEntity> _workingHoursRepository;
    private readonly ILogger<GetWorkingHoursHandler> _logger;

    public GetWorkingHoursHandler(IRepository<WorkingHoursEntity> workingHoursRepository, ILogger<GetWorkingHoursHandler> logger)
    {
        _workingHoursRepository = workingHoursRepository;
        _logger = logger;
    }

    public async Task<Result<List<WorkingHoursDto>>> Handle(GetWorkingHoursQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var spec = new WorkingHoursByPetWalkerSpec(request.PetWalkerId);
            var workingHours = await _workingHoursRepository.ListAsync(spec, cancellationToken);

            _logger.LogInformation("Retrieved {Count} working hours for PetWalker {PetWalkerId}", 
                workingHours.Count, request.PetWalkerId);

            var dtos = workingHours.Select(w => new WorkingHoursDto(
                w.Id,
                w.PetWalkerId,
                w.DayOfWeek,
                w.StartTime,
                w.EndTime,
                w.IsActive
            )).ToList();

            return Result<List<WorkingHoursDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving working hours for PetWalker {PetWalkerId}", request.PetWalkerId);
            return Result<List<WorkingHoursDto>>.Error(ex.Message);
        }
    }
}
