using Ardalis.Result;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using BookingEntity = FurryFriends.Core.BookingAggregate.Booking;
using CustomTimeRequestEntity = FurryFriends.Core.TimeslotAggregate.CustomTimeRequest;

namespace FurryFriends.UseCases.Timeslots.CustomTimeRequest;

internal class RespondToCustomTimeRequestHandler : ICommandHandler<RespondToCustomTimeRequestCommand, Result<CustomTimeRequestDto>>
{
    private readonly IRepository<CustomTimeRequestEntity> _customTimeRequestRepository;
    private readonly IRepository<BookingEntity> _bookingRepository;
    private readonly ILogger<RespondToCustomTimeRequestHandler> _logger;

    public RespondToCustomTimeRequestHandler(
        IRepository<CustomTimeRequestEntity> customTimeRequestRepository,
        IRepository<BookingEntity> bookingRepository,
        ILogger<RespondToCustomTimeRequestHandler> logger)
    {
        _customTimeRequestRepository = customTimeRequestRepository;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<CustomTimeRequestDto>> Handle(RespondToCustomTimeRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get the custom time request
            var requestSpec = new CustomTimeRequestByIdSpec(request.RequestId);
            var customTimeRequest = await _customTimeRequestRepository.FirstOrDefaultAsync(requestSpec, cancellationToken);

            if (customTimeRequest == null)
            {
                return Result<CustomTimeRequestDto>.Error("Custom time request not found");
            }

            // 2. Process the response based on type
            Result result;

            switch (request.Response)
            {
                case CustomTimeRequestResponse.Accept:
                    result = customTimeRequest.Accept();
                    if (!result.IsSuccess)
                    {
                        return Result<CustomTimeRequestDto>.Error(result.Errors.FirstOrDefault() ?? "Failed to accept request");
                    }
                    
                    // Create a booking automatically when accepted
                    await CreateBookingAsync(customTimeRequest, cancellationToken);
                    break;

                case CustomTimeRequestResponse.Decline:
                    result = customTimeRequest.Decline(request.Reason ?? "Declined by petwalker");
                    if (!result.IsSuccess)
                    {
                        return Result<CustomTimeRequestDto>.Error(result.Errors.FirstOrDefault() ?? "Failed to decline request");
                    }
                    break;

                case CustomTimeRequestResponse.CounterOffer:
                    if (!request.CounterOfferedDate.HasValue || !request.CounterOfferedTime.HasValue)
                    {
                        return Result<CustomTimeRequestDto>.Error("Counter-offered date and time are required");
                    }
                    
                    result = customTimeRequest.CounterOffer(
                        request.CounterOfferedDate.Value,
                        request.CounterOfferedTime.Value,
                        request.Reason ?? "Counter-offered");
                    
                    if (!result.IsSuccess)
                    {
                        return Result<CustomTimeRequestDto>.Error(result.Errors.FirstOrDefault() ?? "Failed to counter-offer request");
                    }
                    break;

                default:
                    return Result<CustomTimeRequestDto>.Error("Invalid response type");
            }

            // 3. Update the request in database
            await _customTimeRequestRepository.UpdateAsync(customTimeRequest, cancellationToken);

            _logger.LogInformation(
                "Processed {Response} for custom time request {RequestId}",
                request.Response, request.RequestId);

            // 4. Return DTO
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
            _logger.LogError(ex, "Error processing response for custom time request {RequestId}", request.RequestId);
            return Result<CustomTimeRequestDto>.Error(ex.Message);
        }
    }

    private async Task CreateBookingAsync(CustomTimeRequestEntity customTimeRequest, CancellationToken cancellationToken)
    {
        // Create a booking when the request is accepted
        var startDateTime = customTimeRequest.RequestedDate.ToDateTime(customTimeRequest.PreferredStartTime);
        var endDateTime = customTimeRequest.RequestedDate.ToDateTime(customTimeRequest.PreferredEndTime);

        // Use reflection to create booking since constructor is private
        var booking = (BookingEntity)Activator.CreateInstance(typeof(BookingEntity), true)!;

        // Use reflection to set the properties since they're private
        var baseType = typeof(BookingEntity).BaseType!;
        var idProperty = baseType.GetProperty("Id")!;
        var petWalkerIdProperty = typeof(BookingEntity).GetProperty("PetWalkerId",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!;
        var petOwnerIdProperty = typeof(BookingEntity).GetProperty("PetOwnerId",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!;
        var startTimeProperty = typeof(BookingEntity).GetProperty("StartTime",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!;
        var endTimeProperty = typeof(BookingEntity).GetProperty("EndTime",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!;
        var priceProperty = typeof(BookingEntity).GetProperty("Price",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!;
        var statusProperty = typeof(BookingEntity).GetProperty("Status",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!;

        idProperty.SetValue(booking, Guid.NewGuid());
        petWalkerIdProperty.SetValue(booking, customTimeRequest.PetWalkerId);
        petOwnerIdProperty.SetValue(booking, customTimeRequest.ClientId);
        startTimeProperty.SetValue(booking, startDateTime);
        endTimeProperty.SetValue(booking, endDateTime);
        priceProperty.SetValue(booking, 0m);
        statusProperty.SetValue(booking, BookingStatus.Confirmed);

        // Set timeslot info (using Guid.Empty since there's no actual timeslot for custom requests)
        booking.SetTimeslotInfo(Guid.Empty, customTimeRequest.ClientAddress);

        await _bookingRepository.AddAsync(booking, cancellationToken);

        _logger.LogInformation(
            "Created booking {BookingId} from custom time request {RequestId}",
            booking.Id, customTimeRequest.Id);
    }
}
