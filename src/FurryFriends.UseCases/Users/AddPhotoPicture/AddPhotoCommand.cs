using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCase.Users.AddPhotoPicture;
public record AddPhotoCommand(Guid UserId, Photo BioPicture) : ICommand<Result>;
