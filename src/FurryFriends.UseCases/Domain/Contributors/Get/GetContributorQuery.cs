using FurryFriends.UseCases.Domain.Contributors;

namespace FurryFriends.UseCases.Domain.Contributors.Get;

public record GetContributorQuery(int ContributorId) : IQuery<Result<ContributorDTO>>;
