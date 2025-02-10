using FurryFriends.UseCases.Domain.Contributors;
using FurryFriends.UseCases.Domain.Contributors.List;

namespace FurryFriends.Infrastructure.Data.Queries;

public class ListContributorsQueryService(AppDbContext _db) : IListContributorsQueryService
{

  public async Task<IEnumerable<ContributorDTO>> ListAsync()
  {

    var result = await _db.Contributors.AsNoTracking()
        .ToListAsync();

    var mappedResult = result.Select(c => new ContributorDTO(c.Id, c.Name.FullName, c.PhoneNumber?.Number))
    .ToList();

    return mappedResult;
  }
}
