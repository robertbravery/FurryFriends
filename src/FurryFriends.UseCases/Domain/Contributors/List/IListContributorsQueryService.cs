using FurryFriends.UseCases.Domain.Contributors;

namespace FurryFriends.UseCases.Domain.Contributors.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListContributorsQueryService
{
  Task<IEnumerable<ContributorDTO>> ListAsync();
}
