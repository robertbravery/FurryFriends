using FurryFriends.UseCases.Contributors;

namespace FurryFriends.UseCases.Contributors.Update;

public record UpdateContributorCommand(int ContributorId, string NewName) : ICommand<Result<ContributorDTO>>;
