using FurryFriends.Core.Common;
using FurryFriends.Core.Enums;

namespace FurryFriends.Core.TimeslotAggregate;

public class CustomTimeRequest : AuditableEntity<Guid>
{
    public Guid ClientId { get; private set; }
    public Guid PetWalkerId { get; private set; }
    public DateOnly RequestedDate { get; private set; }
    public TimeOnly PreferredStartTime { get; private set; }
    public TimeOnly PreferredEndTime { get; private set; }
    public int PreferredDurationMinutes { get; private set; }
    public CustomTimeRequestStatus Status { get; private set; }
    public string ClientAddress { get; private set; } = string.Empty;
    public DateOnly? CounterOfferedDate { get; private set; }
    public TimeOnly? CounterOfferedTime { get; private set; }
    public string? ResponseReason { get; private set; }

    internal CustomTimeRequest() { } // Required by EF Core

    private CustomTimeRequest(
        Guid clientId,
        Guid petWalkerId,
        DateOnly requestedDate,
        TimeOnly preferredStartTime,
        int preferredDurationMinutes,
        string clientAddress)
    {
        Id = Guid.NewGuid();
        ClientId = clientId;
        PetWalkerId = petWalkerId;
        RequestedDate = requestedDate;
        PreferredStartTime = preferredStartTime;
        PreferredDurationMinutes = preferredDurationMinutes;
        PreferredEndTime = preferredStartTime.AddMinutes(preferredDurationMinutes);
        Status = CustomTimeRequestStatus.Pending;
        ClientAddress = clientAddress;
    }

    public static Result<CustomTimeRequest> Create(
        Guid clientId,
        Guid petWalkerId,
        DateOnly requestedDate,
        TimeOnly preferredStartTime,
        int preferredDurationMinutes,
        string clientAddress)
    {
        Guard.Against.Default(clientId, nameof(clientId));
        Guard.Against.Default(petWalkerId, nameof(petWalkerId));
        Guard.Against.OutOfRange(requestedDate, nameof(requestedDate), DateOnly.FromDateTime(DateTime.Today), DateOnly.MaxValue);
        Guard.Against.OutOfRange(preferredDurationMinutes, nameof(preferredDurationMinutes), 30, 45);
        Guard.Against.NullOrWhiteSpace(clientAddress, nameof(clientAddress));

        return Result.Success(new CustomTimeRequest(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            preferredDurationMinutes,
            clientAddress));
    }

    public Result Accept()
    {
        if (Status != CustomTimeRequestStatus.Pending)
        {
            return Result.Error("Can only accept a pending request");
        }

        Status = CustomTimeRequestStatus.Accepted;
        UpdatedAt = DateTime.Now;
        return Result.Success();
    }

    public Result Decline(string reason)
    {
        if (Status != CustomTimeRequestStatus.Pending)
        {
            return Result.Error("Can only decline a pending request");
        }

        Status = CustomTimeRequestStatus.Declined;
        ResponseReason = reason;
        UpdatedAt = DateTime.Now;
        return Result.Success();
    }

    public Result CounterOffer(DateOnly counterOfferedDate, TimeOnly counterOfferedTime, string reason)
    {
        if (Status != CustomTimeRequestStatus.Pending)
        {
            return Result.Error("Can only counter-offer a pending request");
        }

        Status = CustomTimeRequestStatus.CounterOffered;
        CounterOfferedDate = counterOfferedDate;
        CounterOfferedTime = counterOfferedTime;
        ResponseReason = reason;
        UpdatedAt = DateTime.Now;
        return Result.Success();
    }

    public void Expire()
    {
        Status = CustomTimeRequestStatus.Expired;
        UpdatedAt = DateTime.Now;
    }
}