using FurryFriends.UseCases.Services.PetWalkerService;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.PetWalkers.ListPetWalker;
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
      //_logger.LogInformation("Retrieving users using specification: {Specification}", userSpecification);
      //var users = await _repository.ListAsync(userSpecification, cancellationToken);
      var users = await _petWalkerService.ListPetWalkersAsync(query);
      if (users == null)
      {
        _logger.LogError("Failed to retrieve users");
        return Result<PetWalkerListDto>.Error("Failed to retrieve users");
      }
      _logger.LogInformation("Retrieving total count...");
      //var totalCountResult = await _repository.CountAsync(userSpecification, cancellationToken);
      return Result<PetWalkerListDto>.Success(users);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while handling ListPetWalkerQuery: {ErrorMessage}", ex.Message);
      return Result<PetWalkerListDto>.Error(ex.Message);
    }
  }
}

