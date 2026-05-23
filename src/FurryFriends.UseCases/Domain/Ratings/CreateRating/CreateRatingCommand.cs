using MediatR;

namespace FurryFriends.UseCases.Domain.Ratings.CreateRating;

public record CreateRatingCommand(Guid PetWalkerId, Guid ClientId, int RatingValue, string? Comment) : IRequest<Result<Guid>>;
