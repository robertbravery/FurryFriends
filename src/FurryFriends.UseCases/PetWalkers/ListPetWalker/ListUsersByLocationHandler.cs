using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Services;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.PetWalkers.ListPetWalker;
public class ListUsersByLocationHandler(IPetWalkerService petWalkerService, ILogger<ListUsersByLocationHandler> logger) : IQueryHandler<ListPetWalkerByLocationQuery, Result<(List<PetWalker> users, int TotalCount)>>
{
  private readonly IPetWalkerService _petWalkerService = petWalkerService;
  private readonly ILogger<ListUsersByLocationHandler> _logger = logger;

  public async Task<Result<(List<PetWalker> users, int TotalCount)>> Handle(ListPetWalkerByLocationQuery query, CancellationToken cancellationToken)
  {
    try
    {

      var users = await _petWalkerService.ListPetWalkersByLocationAsync(query);
      if (users == null)
      {
        _logger.LogError("Failed to retrieve users");
        return Result<(List<PetWalker> users, int totalCount)>.Error("Failed to retrieve users");
      }
      _logger.LogInformation("Retrieving total count...");
      //var totalCountResult = await _repository.CountAsync(userSpecification, cancellationToken);
      return Result<(List<PetWalker> users, int totalCount)>.Success((users.Value.Users, users.Value.TotalCount));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while handling ListUsersQuery: {ErrorMessage}", ex.Message);
      return Result<(List<PetWalker> users, int totalCount)>.Error(ex.Message);
    }
  }
}

