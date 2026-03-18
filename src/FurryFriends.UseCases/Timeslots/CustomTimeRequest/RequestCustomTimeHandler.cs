using Ardalis.Result;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using CustomTimeRequestEntity = FurryFriends.Core.TimeslotAggregate.CustomTimeRequest;

namespace FurryFriends.UseCases.Timeslots.CustomTimeRequest;

internal class RequestCustomTimeHandler : ICommandHandler<RequestCustomTimeCommand, Result<CustomTimeRequestDto>>
{
    private readonly IRepository<PetWalker> _petWalkerRepository;
    private readonly IRepository<CustomTimeRequestEntity> _customTimeRequestRepository;
    private readonly ILogger<RequestCustomTimeHandler> _logger;

    public RequestCustomTimeHandler(
        IRepository<PetWalker> petWalkerRepository,
        IRepository<CustomTimeRequestEntity> customTimeRequestRepository,
        ILogger<RequestCustomTimeHandler> logger)
    {
        _petWalkerRepository = petWalkerRepository;
        _customTimeRequestRepository = customTimeRequestRepository;
        _logger = logger;
    }

    public async Task<Result<CustomTimeRequestDto>> Handle(RequestCustomTimeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate petwalker exists
            var petWalker = await _petWalkerRepository.GetByIdAsync(request.PetWalkerId, cancellationToken);
            if (petWalker == null)
            {
                return Result<CustomTimeRequestDto>.Error("Petwalker not found");
            }

            // 2. Check for duplicate pending requests
            var pendingRequestSpec = new PendingCustomTimeRequestByClientAndPetWalkerSpec(
                request.ClientId, 
                request.PetWalkerId);
            var existingRequests = await _customTimeRequestRepository.ListAsync(pendingRequestSpec, cancellationToken);
            
            if (existingRequests.Any())
            {
                return Result<CustomTimeRequestDto>.Error("You already have a pending custom time request for this petwalker");
            }

            // 3. Create CustomTimeRequest entity
            var result = CustomTimeRequestEntity.Create(
                request.ClientId,
                request.PetWalkerId,
                request.RequestedDate,
                request.PreferredStartTime,
                request.PreferredDurationMinutes,
                request.ClientAddress);

            if (!result.IsSuccess)
            {
                return Result<CustomTimeRequestDto>.Error(result.Errors.FirstOrDefault() ?? "Failed to create custom time request");
            }

            var customTimeRequest = result.Value;

            // 4. Save to database
            await _customTimeRequestRepository.AddAsync(customTimeRequest, cancellationToken);

            _logger.LogInformation(
                "Created custom time request {RequestId} for client {ClientId} with petwalker {PetWalkerId}",
                customTimeRequest.Id, request.ClientId, request.PetWalkerId);

            // 5. Return DTO
            var dto = new CustomTimeRequestDto(
                customTimeRequest.Id,
                customTimeRequest.PetWalkerId,
                customTimeRequest.ClientId,
                customTimeRequest.RequestedDate,
                customTimeRequest.PreferredStartTime,
                customTimeRequest.PreferredEndTime,
                customTimeRequest.PreferredDurationMinutes,
                customTimeRequest.ClientAddress,
                customTimeRequest.Status.ToString(),
                customTimeRequest.CreatedAt);

            return Result<CustomTimeRequestDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating custom time request for client {ClientId}", request.ClientId);
            return Result<CustomTimeRequestDto>.Error(ex.Message);
        }
    }
}
