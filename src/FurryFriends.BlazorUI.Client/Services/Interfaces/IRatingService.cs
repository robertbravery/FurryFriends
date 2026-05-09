namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

public interface IRatingService
{
    Task<RatingSummaryDto?> GetRatingSummaryAsync(Guid petWalkerId);
    Task<PaginatedRatingResponse> GetRatingsAsync(Guid petWalkerId, int page = 1, int pageSize = 10);
    Task<RatingResult> CreateRatingAsync(CreateRatingRequest request);
    Task<RatingResult> UpdateRatingAsync(Guid ratingId, UpdateRatingRequest request);
    Task<RatingResult> DeleteRatingAsync(Guid ratingId, Guid clientId);
}

public record CreateRatingRequest(Guid BookingId, int RatingValue, string? Comment);
public record UpdateRatingRequest(int? RatingValue, string? Comment);

public record RatingDto(
    Guid Id,
    Guid PetWalkerId,
    Guid ClientId,
    int RatingValue,
    string? Comment,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? ClientName);

public record RatingSummaryDto(
    Guid PetWalkerId,
    double AverageRating,
    int TotalRatings,
    List<RatingDto> RecentRatings);

public record PaginatedRatingResponse(
    List<RatingDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public record RatingResult(bool IsSuccess, string? ErrorMessage);
