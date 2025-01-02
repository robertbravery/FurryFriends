using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.UserAggregate.Specifications;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Users.ListUser;
public class ListUsersHandler : IQueryHandler<ListUsersQuery, Result<(List<User> users, int TotalCount)>>
{
  private readonly IRepository<User> _repository;
  private readonly ILogger<ListUsersHandler> _logger;

  public ListUsersHandler(IRepository<User> repository, ILogger<ListUsersHandler> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public async Task<Result<(List<User> users, int TotalCount)>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
  {
    var userSpecification = new ListUserSpecification(query.SearchString, query.PageSize, query.PageNumber);
    try
    {
      _logger.LogInformation("Retrieving users using specification: {Specification}", userSpecification);
      var users = await _repository.ListAsync(userSpecification, cancellationToken);
      if (users == null)
      {
        _logger.LogError("Failed to retrieve users");
        return Result<(List<User> users, int totalCount)>.Error("Failed to retrieve users");
      }
      _logger.LogInformation("Retrieving total count...");
      var totalCountResult = await _repository.CountAsync(userSpecification, cancellationToken);
      return Result<(List<User> users, int totalCount)>.Success((users, totalCountResult));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while handling ListUsersQuery: {ErrorMessage}", ex.Message);
      return Result<(List<User> users, int totalCount)>.Error(ex.Message);
    }
  }
}

