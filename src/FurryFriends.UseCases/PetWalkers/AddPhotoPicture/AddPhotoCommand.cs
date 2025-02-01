using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCases.PetWalkers.AddPhotoPicture;
public record AddPhotoCommand(Guid UserId, Photo BioPicture) : ICommand<Result>;
