using FurryFriends.UseCase.Contributors;
using FurryFriends.UseCase.Contributors.List;

namespace FurryFriends.Infrastructure.Data.Queries;

public class ListContributorsQueryService(AppDbContext _db) : IListContributorsQueryService
{
  // You can use EF, Dapper, SqlClient, etc. for queries -
  // this is just an example

  public async Task<IEnumerable<ContributorDTO>> ListAsync()
  {

    var result = await _db.Contributors.AsNoTracking()
        .ToListAsync();

    var mappedResult = result.Select(c => new ContributorDTO(c.Id, c.Name.FullName, c.PhoneNumber?.Number))
    .ToList();

    return mappedResult;
  }
}
