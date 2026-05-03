using MediatR;

namespace FurryFriends.UseCases.Domain.Ratings.CreateRating;

public record CreateRatingCommand(Guid BookingId, int RatingValue, string? Comment) : IRequest<Result<Guid>>;
