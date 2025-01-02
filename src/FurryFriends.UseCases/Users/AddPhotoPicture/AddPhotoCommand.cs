using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Users.AddPhotoPicture;
public record AddPhotoCommand(Guid UserId, Photo BioPicture) : ICommand<Result>;
