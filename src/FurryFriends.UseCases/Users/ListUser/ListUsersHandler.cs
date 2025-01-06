using FurryFriends.UseCases.Services;
using FurryFriends.UseCases.Services.DataTransferObjects;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Users.ListUser;
public class ListUsersHandler : IQueryHandler<ListUsersQuery, Result<UserListDto>>
{
  private readonly IUserService _userService;
  private readonly ILogger<ListUsersHandler> _logger;

  public ListUsersHandler(IUserService userService, ILogger<ListUsersHandler> logger)
  {
    _userService = userService;
    _logger = logger;
  }

  public async Task<Result<UserListDto>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
  {
    try
    {
      //_logger.LogInformation("Retrieving users using specification: {Specification}", userSpecification);
      //var users = await _repository.ListAsync(userSpecification, cancellationToken);
      var users = await _userService.ListUsersAsync(query);
      if (users == null)
      {
        _logger.LogError("Failed to retrieve users");
        return Result<UserListDto>.Error("Failed to retrieve users");
      }
      _logger.LogInformation("Retrieving total count...");
      //var totalCountResult = await _repository.CountAsync(userSpecification, cancellationToken);
      return Result<UserListDto>.Success(users);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while handling ListUsersQuery: {ErrorMessage}", ex.Message);
      return Result<UserListDto>.Error(ex.Message);
    }
  }
}

