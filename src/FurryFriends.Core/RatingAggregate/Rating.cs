using Ardalis.SharedKernel;
using FurryFriends.Core.Common;

namespace FurryFriends.Core.RatingAggregate;

public class Rating : BaseEntity<Guid>, IAggregateRoot
{
    public Guid PetWalkerId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid BookingId { get; private set; }
    public int RatingValue { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? ModifiedDate { get; private set; }

    private Rating() { }

    private Rating(Guid petWalkerId, Guid clientId, Guid bookingId, int ratingValue, string? comment)
    {
        Id = Guid.NewGuid();
        PetWalkerId = petWalkerId;
        ClientId = clientId;
        BookingId = bookingId;
        RatingValue = ratingValue;
        Comment = comment;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = null;
    }

    public static Rating Create(Guid petWalkerId, Guid clientId, Guid bookingId, int ratingValue, string? comment)
    {
        Guard.Against.NullOrEmpty(petWalkerId, nameof(petWalkerId));
        Guard.Against.NullOrEmpty(clientId, nameof(clientId));
        Guard.Against.NullOrEmpty(bookingId, nameof(bookingId));
        Guard.Against.OutOfRange(ratingValue, nameof(ratingValue), 1, 5);

        return new Rating(petWalkerId, clientId, bookingId, ratingValue, comment) { Id = Guid.NewGuid() };
    }

    public bool CanUpdate()
    {
        return CreatedDate.AddDays(7) > DateTime.UtcNow && ModifiedDate == null;
    }

    public void UpdateRatingValue(int ratingValue)
    {
        if (!CanUpdate())
        {
            throw new InvalidOperationException("Rating cannot be updated. Either 7 days have passed or it has already been updated.");
        }
        Guard.Against.OutOfRange(ratingValue, nameof(ratingValue), 1, 5);
        RatingValue = ratingValue;
        ModifiedDate = DateTime.UtcNow;
    }

    public void UpdateComment(string? comment)
    {
        if (!CanUpdate())
        {
            throw new InvalidOperationException("Rating cannot be updated. Either 7 days have passed or it has already been updated.");
        }
        Comment = comment;
        ModifiedDate = DateTime.UtcNow;
    }
}