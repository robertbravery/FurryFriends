using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.AddPhotoPicture;
public record AddPhotoCommand(Guid UserId, Photo BioPicture) : ICommand<Result>;
