using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.UseCases.Domain.PetWalkers.Query.ListPetWalker;

namespace FurryFriends.UseCases.Services.PetWalkerService;

public interface IPetWalkerService
{
  Task<Result<PetWalker>> CreatePetWalkerAsync(CreatePetWalkerDto dto);
  Task<Result<PetWalker>> UpdatePetWalkerAsync(UpdatePetWalkerDto dto, CancellationToken cancellationToken);
  Task AddBioPictureAsync(Photo photo, Guid userId);
  Task<Result<PetWalker>> GetPetWalkerByEmailAsync(string email, CancellationToken cancellationToken);
  Task<Result<PetWalkerListDto>> ListPetWalkersAsync(ListPetWalkerQuery query);
  Task<Result<PetWalkerListDto>> ListPetWalkersByLocationAsync(ListPetWalkerByLocationQuery query);
  Task<Result> UpdatePetWalkerHourlyRateAsync(Guid userId, decimal hourlyRate, string currency, CancellationToken cancellationToken);
}
