using FurryFriends.Core.Common;

namespace FurryFriends.Core.RatingAggregate;

public class Rating : BaseEntity<Guid>
{
    public Guid PetWalkerId { get; private set; }
    public Guid ClientId { get; private set; }
    public int RatingValue { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedDate { get; private set; }

    private Rating() { }

    private Rating(Guid petWalkerId, Guid clientId, int ratingValue, string? comment)
    {
        Id = Guid.NewGuid();
        PetWalkerId = petWalkerId;
        ClientId = clientId;
        RatingValue = ratingValue;
        Comment = comment;
        CreatedDate = DateTime.UtcNow;
    }

    public static Rating Create(Guid petWalkerId, Guid clientId, int ratingValue, string? comment)
    {
        Guard.Against.NullOrEmpty(petWalkerId, nameof(petWalkerId));
        Guard.Against.NullOrEmpty(clientId, nameof(clientId));
        Guard.Against.OutOfRange(ratingValue, nameof(ratingValue), 1, 5);

        return new Rating(petWalkerId, clientId, ratingValue, comment) { Id = Guid.NewGuid() };
    }

    public void UpdateRatingValue(int ratingValue)
    {
        Guard.Against.OutOfRange(ratingValue, nameof(ratingValue), 1, 5);
        RatingValue = ratingValue;
    }

    public void UpdateComment(string? comment)
    {
        Comment = comment;
    }
}