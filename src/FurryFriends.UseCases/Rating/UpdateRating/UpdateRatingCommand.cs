using Ardalis.Result;
using MediatR;

namespace FurryFriends.UseCases.Rating.UpdateRating;

public record UpdateRatingCommand(Guid RatingId, int? RatingValue, string? Comment) : IRequest<Result<Guid>>;
