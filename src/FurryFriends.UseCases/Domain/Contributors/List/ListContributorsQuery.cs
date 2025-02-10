using FurryFriends.UseCases.Domain.Contributors;

namespace FurryFriends.UseCases.Domain.Contributors.List;

public record ListContributorsQuery(int? Skip, int? Take) : IQuery<Result<IEnumerable<ContributorDTO>>>;
