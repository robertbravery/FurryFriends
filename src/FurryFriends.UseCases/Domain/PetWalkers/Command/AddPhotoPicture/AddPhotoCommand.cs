using FurryFriends.Core.PetWalkerAggregate;
using Mediator;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.AddPhotoPicture;
public record AddPhotoCommand(Guid UserId, Photo BioPicture) : ICommand<Result>;
