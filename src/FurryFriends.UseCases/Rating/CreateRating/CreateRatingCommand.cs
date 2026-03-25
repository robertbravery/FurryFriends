using Ardalis.Result;
using MediatR;

namespace FurryFriends.UseCases.Rating.CreateRating;

public record CreateRatingCommand(Guid BookingId, int RatingValue, string? Comment) : IRequest<Result<Guid>>;
