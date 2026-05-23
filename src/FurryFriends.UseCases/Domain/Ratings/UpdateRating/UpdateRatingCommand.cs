using MediatR;

namespace FurryFriends.UseCases.Domain.Ratings.UpdateRating;

public record UpdateRatingCommand(Guid RatingId, int? RatingValue, string? Comment) : IRequest<Result<Guid>>;
