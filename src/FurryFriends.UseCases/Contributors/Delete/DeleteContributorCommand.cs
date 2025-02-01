namespace FurryFriends.UseCase.Contributors.Delete;

public record DeleteContributorCommand(int ContributorId) : ICommand<Result>;
