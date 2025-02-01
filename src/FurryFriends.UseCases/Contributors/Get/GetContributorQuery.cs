using FurryFriends.UseCases.Contributors;

namespace FurryFriends.UseCases.Contributors.Get;

public record GetContributorQuery(int ContributorId) : IQuery<Result<ContributorDTO>>;
