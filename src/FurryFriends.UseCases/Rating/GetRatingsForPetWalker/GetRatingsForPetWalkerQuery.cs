using Ardalis.Result;
using MediatR;

namespace FurryFriends.UseCases.Rating.GetRatingsForPetWalker;

public record GetRatingsForPetWalkerQuery(Guid PetWalkerId, int Page = 1, int PageSize = 20) : IRequest<Result<List<RatingDto>>>;
