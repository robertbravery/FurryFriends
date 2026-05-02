using FurryFriends.Core.Common;
using FurryFriends.Core.PetWalkerAggregate.Events;

namespace FurryFriends.Core.RatingAggregate;

public class Rating : AuditableEntity<Guid>
{
    public Guid PetWalkerId { get; private set; }
    public Guid ClientId { get; private set; }
    public int RatingValue { get; private set; }
    public string? Comment { get; private set; }
    public RatingStatus Status { get; private set; }

    private Rating() { }

    private Rating(Guid petWalkerId, Guid clientId, int ratingValue, string? comment)
    {
        Id = Guid.NewGuid();
        PetWalkerId = petWalkerId;
        ClientId = clientId;
        RatingValue = ratingValue;
        Comment = comment;
        Status = RatingStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Rating Create(Guid petWalkerId, Guid clientId, int ratingValue, string? comment)
    {
        Guard.Against.Default(petWalkerId, nameof(petWalkerId));
        Guard.Against.Default(clientId, nameof(clientId));
        Guard.Against.OutOfRange(ratingValue, nameof(ratingValue), 1, 5);

        var rating = new Rating(petWalkerId, clientId, ratingValue, comment);
        rating.RegisterDomainEvent(new RatingAddedEvent(rating));

        return rating;
    }

    public bool CanEdit()
    {
        return CreatedAt.AddHours(24) > DateTime.UtcNow;
    }

    public Result UpdateRating(int ratingValue, string? comment)
    {
        if (!CanEdit())
        {
            return Result.Error("Rating can only be edited within 24 hours of submission.");
        }

        if (Status != RatingStatus.Active)
        {
            return Result.Error("Rating cannot be modified because it is under moderation review.");
        }

        Guard.Against.OutOfRange(ratingValue, nameof(ratingValue), 1, 5);

        RatingValue = ratingValue;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;

        RegisterDomainEvent(new RatingUpdatedEvent(this));

        return Result.Success();
    }

    public Result Remove()
    {
        if (!CanEdit())
        {
            return Result.Error("Rating can only be removed within 24 hours of submission.");
        }

        if (Status != RatingStatus.Active)
        {
            return Result.Error("Rating cannot be removed because it is under moderation review.");
        }

        Status = RatingStatus.Removed;
        UpdatedAt = DateTime.UtcNow;

        RegisterDomainEvent(new RatingRemovedEvent(this));

        return Result.Success();
    }

    public void SetStatus(RatingStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}