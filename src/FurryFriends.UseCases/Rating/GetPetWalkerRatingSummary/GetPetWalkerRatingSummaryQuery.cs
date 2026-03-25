using Ardalis.Result;
using MediatR;

namespace FurryFriends.UseCases.Rating.GetPetWalkerRatingSummary;

public record GetPetWalkerRatingSummaryQuery(Guid PetWalkerId) : IRequest<Result<PetWalkerRatingSummaryDto>>;
