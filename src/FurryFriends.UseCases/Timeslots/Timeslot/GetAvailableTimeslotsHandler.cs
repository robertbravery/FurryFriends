using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

internal class GetAvailableTimeslotsHandler : IQueryHandler<GetAvailableTimeslotsQuery, Result<GetAvailableTimeslotsResponse>>
{
    private readonly IRepository<TimeslotEntity> _timeslotRepository;
    private readonly IRepository<PetWalker> _petWalkerRepository;
    private readonly ILogger<GetAvailableTimeslotsHandler> _logger;

    public GetAvailableTimeslotsHandler(
        IRepository<TimeslotEntity> timeslotRepository,
        IRepository<PetWalker> petWalkerRepository,
        ILogger<GetAvailableTimeslotsHandler> logger)
    {
        _timeslotRepository = timeslotRepository;
        _petWalkerRepository = petWalkerRepository;
        _logger = logger;
    }

    public async Task<Result<GetAvailableTimeslotsResponse>> Handle(GetAvailableTimeslotsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Getting available timeslots for PetWalker: {PetWalkerId}, Date: {Date}",
                request.PetWalkerId,
                request.Date);

            // Check if PetWalker exists
            var petWalkerSpec = new GetPetWalkerByIdSpecification(request.PetWalkerId);
            var petWalker = await _petWalkerRepository.FirstOrDefaultAsync(petWalkerSpec, cancellationToken);

            if (petWalker == null)
            {
                _logger.LogWarning("PetWalker not found: {PetWalkerId}", request.PetWalkerId);
                return Result.NotFound("PetWalker not found");
            }

            // Get available timeslots for the specified date
            var spec = new AvailableTimeslotsByPetWalkerAndDateSpec(request.PetWalkerId, request.Date);
            var timeslots = await _timeslotRepository.ListAsync(spec, cancellationToken);

            // Filter out past timeslots (if date is today, filter out past start times)
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            var today = DateOnly.FromDateTime(DateTime.Today);

            var availableTimeslots = timeslots
                .Where(t => request.Date > today || (request.Date == today && t.StartTime > currentTime))
                .Select(t => new AvailableTimeslotDto(
                    t.Id,
                    t.StartTime,
                    t.EndTime,
                    t.DurationInMinutes))
                .ToList();

            // Check for travel buffer settings (simplified - no travel buffer property on PetWalker yet)
            var hasTravelBuffer = false;
            string? travelBufferMessage = null;

            var response = new GetAvailableTimeslotsResponse(
                request.PetWalkerId,
                request.Date,
                availableTimeslots,
                hasTravelBuffer,
                travelBufferMessage);

            _logger.LogInformation(
                "Found {Count} available timeslots for PetWalker: {PetWalkerId}, Date: {Date}",
                availableTimeslots.Count,
                request.PetWalkerId,
                request.Date);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available timeslots for PetWalker: {PetWalkerId}, Date: {Date}",
                request.PetWalkerId,
                request.Date);
            return Result.Error($"An error occurred while getting available timeslots: {ex.Message}");
        }
    }
}
