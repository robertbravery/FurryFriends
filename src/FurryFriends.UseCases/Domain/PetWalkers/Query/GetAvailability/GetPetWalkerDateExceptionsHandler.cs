using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetAvailability;

public class GetPetWalkerDateExceptionsHandler
    : IQueryHandler<GetPetWalkerDateExceptionsQuery, Result<List<PetWalkerDateExceptionDto>>>
{
  private readonly IRepository<PetWalkerDateException> _exceptionRepo;

  public GetPetWalkerDateExceptionsHandler(IRepository<PetWalkerDateException> exceptionRepo)
  {
    _exceptionRepo = exceptionRepo;
  }

  public async Task<Result<List<PetWalkerDateExceptionDto>>> Handle(
      GetPetWalkerDateExceptionsQuery request,
      CancellationToken cancellationToken)
  {
    if (request.PetWalkerId == Guid.Empty)
    {
      return Result<List<PetWalkerDateExceptionDto>>.Error("PetWalker ID is required");
    }

    var exceptions = await _exceptionRepo.ListAsync(
        new PetWalkerDateExceptionsByPetWalkerSpec(request.PetWalkerId),
        cancellationToken);

    return Result<List<PetWalkerDateExceptionDto>>.Success(
        exceptions.Select(e => e.ToDto()).ToList());
  }
}
