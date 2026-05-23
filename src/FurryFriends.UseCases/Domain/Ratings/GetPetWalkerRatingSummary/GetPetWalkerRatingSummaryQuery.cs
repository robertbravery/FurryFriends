using MediatR;

namespace FurryFriends.UseCases.Domain.Ratings.GetPetWalkerRatingSummary;

public record GetPetWalkerRatingSummaryQuery(Guid PetWalkerId) : IRequest<Result<PetWalkerRatingSummaryDto>>;
