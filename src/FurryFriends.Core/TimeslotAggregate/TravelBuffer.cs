using FurryFriends.Core.Common;

namespace FurryFriends.Core.TimeslotAggregate;

public class TravelBuffer : AuditableEntity<Guid>
{
    public Guid BookingId { get; private set; }
    public Guid? PreviousBookingId { get; private set; }
    public string OriginAddress { get; private set; } = string.Empty;
    public string DestinationAddress { get; private set; } = string.Empty;
    public int BufferDurationMinutes { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    internal TravelBuffer() { } // Required by EF Core

    private TravelBuffer(
        Guid bookingId,
        string originAddress,
        string destinationAddress,
        int bufferDurationMinutes,
        DateTime startTime,
        Guid? previousBookingId = null)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        PreviousBookingId = previousBookingId;
        OriginAddress = originAddress;
        DestinationAddress = destinationAddress;
        BufferDurationMinutes = bufferDurationMinutes;
        StartTime = startTime;
        EndTime = startTime.AddMinutes(bufferDurationMinutes);
    }

    public static Result<TravelBuffer> Create(
        Guid bookingId,
        string originAddress,
        string destinationDescription,
        int bufferDurationMinutes,
        DateTime startTime,
        Guid? previousBookingId = null)
    {
        Guard.Against.Default(bookingId, nameof(bookingId));
        Guard.Against.NullOrWhiteSpace(originAddress, nameof(originAddress));
        Guard.Against.StringTooLong(originAddress, 500, nameof(originAddress));
        Guard.Against.NullOrWhiteSpace(destinationDescription, nameof(destinationDescription));
        Guard.Against.StringTooLong(destinationDescription, 500, nameof(destinationDescription));
        Guard.Against.NegativeOrZero(bufferDurationMinutes, nameof(bufferDurationMinutes));

        return Result.Success(new TravelBuffer(
            bookingId,
            originAddress,
            destinationDescription,
            bufferDurationMinutes,
            startTime,
            previousBookingId));
    }

    // Overload for compatibility with different parameter naming
    public static Result<TravelBuffer> Create(
        Guid bookingId,
        string originAddress,
        string destinationAddress,
        string destinationDescription,
        int bufferDurationMinutes,
        DateTime startTime,
        Guid? previousBookingId = null)
    {
        return Create(bookingId, originAddress, destinationDescription ?? destinationAddress, bufferDurationMinutes, startTime, previousBookingId);
    }
}