namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

public interface IRatingService
{
    Task<RatingSummaryDto?> GetRatingSummaryAsync(Guid petWalkerId);
    Task<List<RatingDto>> GetRatingsAsync(Guid petWalkerId, int page = 1, int pageSize = 20);
    Task<bool> CreateRatingAsync(CreateRatingRequest request);
    Task<bool> UpdateRatingAsync(Guid ratingId, UpdateRatingRequest request);
}

public record CreateRatingRequest(Guid BookingId, int RatingValue, string? Comment);
public record UpdateRatingRequest(int? RatingValue, string? Comment);

public record RatingDto(
    Guid Id,
    Guid PetWalkerId,
    Guid ClientId,
    Guid BookingId,
    int RatingValue,
    string? Comment,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    string? ClientName);

public record RatingSummaryDto(
    Guid PetWalkerId,
    double AverageRating,
    int TotalRatings,
    List<RatingDto> RecentRatings);
