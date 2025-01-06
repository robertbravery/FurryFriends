using FurryFriends.Core.UserAggregate;
using FurryFriends.UseCases.Services;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Users.ListUser;
public class ListUsersByLocationHandler : IQueryHandler<ListUsersByLocationQuery, Result<(List<PetWalker> users, int TotalCount)>>
{
  private readonly IUserService _userService;
  private readonly ILogger<ListUsersByLocationHandler> _logger;

  public ListUsersByLocationHandler(IUserService userService, ILogger<ListUsersByLocationHandler> logger)
  {
    _userService = userService;
    _logger = logger;
  }

  public async Task<Result<(List<PetWalker> users, int TotalCount)>> Handle(ListUsersByLocationQuery query, CancellationToken cancellationToken)
  {
    try
    {

      var users = await _userService.ListUserUserByLocationAsync(query);
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

