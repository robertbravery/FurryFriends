using FurryFriends.UseCases.Contributors;

namespace FurryFriends.UseCases.Contributors.List;

public record ListContributorsQuery(int? Skip, int? Take) : IQuery<Result<IEnumerable<ContributorDTO>>>;
