using FurryFriends.UseCases.Services.PetWalkerService;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.ListPetWalker;
public class ListPetWalkersHandler : IQueryHandler<ListPetWalkerQuery, Result<PetWalkerListDto>>
{
  private readonly IPetWalkerService _petWalkerService;
  private readonly ILogger<ListPetWalkersHandler> _logger;

  public ListPetWalkersHandler(IPetWalkerService petWalkerService, ILogger<ListPetWalkersHandler> logger)
  {
    _petWalkerService = petWalkerService;
    _logger = logger;
  }

  public async Task<Result<PetWalkerListDto>> Handle(ListPetWalkerQuery query, CancellationToken cancellationToken)
  {
    try
    {
      var users = await _petWalkerService.ListPetWalkersAsync(query);
      if (users == null)
      {
        _logger.LogError("Failed to retrieve users");
        return Result<PetWalkerListDto>.Error("Failed to retrieve users");
      }
      return Result<PetWalkerListDto>.Success(users);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while handling ListPetWalkerQuery: {ErrorMessage}", ex.Message);
      return Result<PetWalkerListDto>.Error(ex.Message);
    }
  }
}

