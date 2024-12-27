using FurryFriends.Core.Entities;
using FurryFriends.Core.UserAggregate.Specifications;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Users.List;
public class ListUsersHandler : IQueryHandler<ListUsersQuery, Result<(List<User> Users, int TotalCount)>>
{
  private readonly IRepository<User> _repository;
  private readonly ILogger<ListUsersHandler> _logger;

  public ListUsersHandler(IRepository<User> repository, ILogger<ListUsersHandler> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public async Task<Result<(List<User> Users, int TotalCount)>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
  {
    var spec = new ListUserSpec(query.SearchString, query.PageSize, query.PageNumber);
    try
    {
      _logger.LogInformation("Attempting to retrieve users using specification: {Specification}", spec);
      var users = await _repository.ListAsync(spec, cancellationToken);
      if (users == null)
      {
        _logger.LogError("Failed to retrieve users");
        return Result<(List<User> Users, int TotalCount)>.Error("Failed to retrieve users");
      }
      _logger.LogInformation("Successfully retrieved users. Retrieving total count...");
      var totalCount = await _repository.CountAsync(spec, cancellationToken);
      return Result<(List<User> Users, int TotalCount)>.Success((users, totalCount));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while handling ListUsersQuery: {ErrorMessage}", ex.Message);
      return Result<(List<User> Users, int TotalCount)>.Error(ex.Message);
    }
  }
}

