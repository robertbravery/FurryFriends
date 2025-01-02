using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Users.AddBioPicture;
public record AddPhotoCommand(Guid UserId, Photo BioPicture) : ICommand<Result>;
