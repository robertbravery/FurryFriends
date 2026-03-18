using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

internal class GetTimeslotsHandler : IQueryHandler<GetTimeslotsQuery, Result<GetTimeslotsResponse>>
{
    private readonly IRepository<TimeslotEntity> _timeslotRepository;
    private readonly IRepository<PetWalker> _petWalkerRepository;
    private readonly ILogger<GetTimeslotsHandler> _logger;

    public GetTimeslotsHandler(
        IRepository<TimeslotEntity> timeslotRepository,
        IRepository<PetWalker> petWalkerRepository,
        ILogger<GetTimeslotsHandler> logger)
    {
        _timeslotRepository = timeslotRepository;
        _petWalkerRepository = petWalkerRepository;
        _logger = logger;
    }

    public async Task<Result<GetTimeslotsResponse>> Handle(GetTimeslotsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Getting timeslots for PetWalker: {PetWalkerId}, Date: {Date}",
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

            // Get timeslots
            List<TimeslotEntity> timeslots;

            if (request.Date.HasValue)
            {
                // Get timeslots for specific date
                var spec = new TimeslotsByPetWalkerAndDateSpec(request.PetWalkerId, request.Date.Value);
                timeslots = await _timeslotRepository.ListAsync(spec, cancellationToken);
            }
            else
            {
                // Get all timeslots for the pet walker
                var spec = new TimeslotsByPetWalkerSpec(request.PetWalkerId);
                timeslots = await _timeslotRepository.ListAsync(spec, cancellationToken);
            }

            var timeslotDtos = timeslots
                .Select(t => new TimeslotDto(
                    t.Id,
                    t.PetWalkerId,
                    t.Date,
                    t.StartTime,
                    t.EndTime,
                    t.DurationInMinutes,
                    t.Status))
                .ToList();

            var response = new GetTimeslotsResponse(
                request.PetWalkerId,
                request.Date,
                timeslotDtos);

            _logger.LogInformation(
                "Found {Count} timeslots for PetWalker: {PetWalkerId}",
                timeslotDtos.Count,
                request.PetWalkerId);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting timeslots for PetWalker: {PetWalkerId}", request.PetWalkerId);
            return Result.Error($"An error occurred while getting timeslots: {ex.Message}");
        }
    }
}
