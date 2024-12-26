using FurryFriends.Core.Entities;
using FurryFriends.Core.UserAggregate.Specifications;

namespace FurryFriends.UseCases.Users.List;
public class ListUsersHandler : IQueryHandler<ListUsersQuery, Result<(List<User> Users, int TotalCount)>>
{
  private readonly IRepository<User> _repository;

  public ListUsersHandler(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task<Result<(List<User> Users, int TotalCount)>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
  {
    var spec = new ListUserSpec(query.SearchString, query.PageSize, query.PageNumber);
    var users = await _repository.ListAsync(spec, cancellationToken);
    var totalCount = await _repository.CountAsync(spec, cancellationToken);

    return Result<(List<User> Users, int TotalCount)>.Success((users, totalCount));
  }
}

