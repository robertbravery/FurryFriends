using FurryFriends.UseCases.Domain.Contributors;

namespace FurryFriends.UseCases.Domain.Contributors.Update;

public record UpdateContributorCommand(int ContributorId, string NewName) : ICommand<Result<ContributorDTO>>;
