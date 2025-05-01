using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using Microsoft.AspNetCore.Http;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;

public class UpdateBioPictureCommand : ICommand<Result<PhotoDto>>
{
  public required Guid PetWalkerId { get; set; }
  public required IFormFile File { get; set; }
}

