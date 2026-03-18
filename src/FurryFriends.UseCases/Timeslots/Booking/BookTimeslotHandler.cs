using Ardalis.Result;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.BookingAggregate.Specifications;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using BookingEntity = FurryFriends.Core.BookingAggregate.Booking;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;
using TravelBufferEntity = FurryFriends.Core.TimeslotAggregate.TravelBuffer;

namespace FurryFriends.UseCases.Timeslots.Booking;

internal class BookTimeslotHandler : ICommandHandler<BookTimeslotCommand, Result<BookTimeslotDto>>
{
    private readonly IRepository<TimeslotEntity> _timeslotRepository;
    private readonly IRepository<BookingEntity> _bookingRepository;
    private readonly IRepository<Client> _clientRepository;
    private readonly IRepository<TravelBufferEntity> _travelBufferRepository;
    private readonly TravelBufferCalculator _travelBufferCalculator;
    private readonly ILogger<BookTimeslotHandler> _logger;

    public BookTimeslotHandler(
        IRepository<TimeslotEntity> timeslotRepository,
        IRepository<BookingEntity> bookingRepository,
        IRepository<Client> clientRepository,
        IRepository<TravelBufferEntity> travelBufferRepository,
        TravelBufferCalculator travelBufferCalculator,
        ILogger<BookTimeslotHandler> logger)
    {
        _timeslotRepository = timeslotRepository;
        _bookingRepository = bookingRepository;
        _clientRepository = clientRepository;
        _travelBufferRepository = travelBufferRepository;
        _travelBufferCalculator = travelBufferCalculator;
        _logger = logger;
    }

    public async Task<Result<BookTimeslotDto>> Handle(BookTimeslotCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get the timeslot
            var timeslotSpec = new TimeslotByIdSpec(request.TimeslotId);
            var timeslot = await _timeslotRepository.FirstOrDefaultAsync(timeslotSpec, cancellationToken);

            if (timeslot == null)
            {
                return Result<BookTimeslotDto>.Error("Timeslot not found");
            }

            // 2. Verify timeslot is available
            if (timeslot.Status != TimeslotStatus.Available)
            {
                return Result<BookTimeslotDto>.Error("This timeslot is no longer available");
            }

            // 3. Verify client exists
            var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
            if (client == null)
            {
                return Result<BookTimeslotDto>.Error("Client not found");
            }

            // 4. Create booking (atomic with timeslot status update)
            var startDateTime = timeslot.Date.ToDateTime(timeslot.StartTime);
            var endDateTime = timeslot.Date.ToDateTime(timeslot.EndTime);

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
            petWalkerIdProperty.SetValue(booking, timeslot.PetWalkerId);
            petOwnerIdProperty.SetValue(booking, request.ClientId);
            startTimeProperty.SetValue(booking, startDateTime);
            endTimeProperty.SetValue(booking, endDateTime);
            priceProperty.SetValue(booking, 0m);
            statusProperty.SetValue(booking, BookingStatus.Confirmed);

            // Set timeslot info
            booking.SetTimeslotInfo(timeslot.Id, request.ClientAddress);

            await _bookingRepository.AddAsync(booking, cancellationToken);

            // 5. Update timeslot status to Booked
            timeslot.Book();
            await _timeslotRepository.UpdateAsync(timeslot, cancellationToken);

            // 6. Calculate travel buffer if there's a previous booking for this petwalker on the same day
            int? bufferMinutes = null;
            var previousBooking = await GetPreviousBookingForPetWalkerToday(
                timeslot.PetWalkerId, 
                timeslot.Date, 
                cancellationToken);

            if (previousBooking != null && previousBooking.ClientAddress != request.ClientAddress)
            {
                // Calculate travel buffer
                bufferMinutes = _travelBufferCalculator.CalculateBufferMinutes(
                    previousBooking.ClientAddress, 
                    request.ClientAddress);

                // Create travel buffer entity
                var bufferStartTime = timeslot.Date.ToDateTime(timeslot.EndTime);
                var bufferResult = TravelBufferEntity.Create(
                    booking.Id,
                    previousBooking.ClientAddress,
                    request.ClientAddress,
                    bufferMinutes.Value,
                    bufferStartTime);

                if (bufferResult.IsSuccess)
                {
                    var travelBuffer = bufferResult.Value;
                    await _travelBufferRepository.AddAsync(travelBuffer, cancellationToken);

                    // Block subsequent timeslots during buffer period
                    await BlockTimeslotsDuringBuffer(
                        timeslot.PetWalkerId,
                        timeslot.Date,
                        timeslot.EndTime,
                        bufferMinutes.Value,
                        cancellationToken);
                }
            }

            _logger.LogInformation(
                "Booked timeslot {TimeslotId} for client {ClientId} with booking {BookingId}",
                timeslot.Id, request.ClientId, booking.Id);

            var dto = new BookTimeslotDto(
                booking.Id,
                timeslot.Id,
                timeslot.PetWalkerId,
                request.ClientId,
                timeslot.Date,
                timeslot.StartTime,
                timeslot.EndTime,
                request.ClientAddress,
                "Confirmed",
                bufferMinutes.HasValue,
                bufferMinutes);

            return Result<BookTimeslotDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking timeslot {TimeslotId}", request.TimeslotId);
            return Result<BookTimeslotDto>.Error(ex.Message);
        }
    }

    private async Task<BookingEntity?> GetPreviousBookingForPetWalkerToday(
        Guid petWalkerId, 
        DateOnly date, 
        CancellationToken cancellationToken)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

        var spec = new BookingByDateSpec(petWalkerId, startOfDay, endOfDay);
        var bookings = await _bookingRepository.ListAsync(spec, cancellationToken);
        
        return bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .OrderBy(b => b.StartTime)
            .FirstOrDefault();
    }

    private async Task BlockTimeslotsDuringBuffer(
        Guid petWalkerId,
        DateOnly date,
        TimeOnly timeslotEndTime,
        int bufferMinutes,
        CancellationToken cancellationToken)
    {
        var bufferSpec = new TimeslotsDuringBufferSpec(petWalkerId, date, timeslotEndTime, bufferMinutes);
        var timeslotsToBlock = await _timeslotRepository.ListAsync(bufferSpec, cancellationToken);

        foreach (var ts in timeslotsToBlock)
        {
            ts.MakeUnavailable();
            await _timeslotRepository.UpdateAsync(ts, cancellationToken);
            
            _logger.LogInformation(
                "Blocked timeslot {TimeslotId} due to travel buffer",
                ts.Id);
        }
    }
}
