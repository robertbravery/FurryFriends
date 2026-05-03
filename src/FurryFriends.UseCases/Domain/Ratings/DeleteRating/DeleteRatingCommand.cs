using MediatR;

namespace FurryFriends.UseCases.Domain.Ratings.DeleteRating;

public record DeleteRatingCommand(Guid RatingId, Guid ClientId) : IRequest<Result>;
