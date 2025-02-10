namespace FurryFriends.UseCases.Domain.Contributors.Delete;

public record DeleteContributorCommand(int ContributorId) : ICommand<Result>;
