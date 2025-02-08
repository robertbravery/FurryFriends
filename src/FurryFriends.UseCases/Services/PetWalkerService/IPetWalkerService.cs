using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.PetWalkers.CreatePetWalker;
using FurryFriends.UseCases.PetWalkers.ListPetWalker;

namespace FurryFriends.UseCases.Services.PetWalkerService;

public interface IPetWalkerService
{
  Task<Result<PetWalker>> CreatePetWalkerAsync(CreatePetWalkerDto dto);
  Task AddBioPictureAsync(Photo photo, Guid userId);
  Task<Result<PetWalker>> GetPetWalkerByEmailAsync(string email, CancellationToken cancellationToken);
  Task<Result<PetWalkerListDto>> ListPetWalkersAsync(ListPetWalkerQuery query);
  Task<Result<PetWalkerListDto>> ListPetWalkersByLocationAsync(ListPetWalkerByLocationQuery query);
  Task<Result> UpdatePetWalkerHourlyRateAsync(Guid userId, decimal hourlyRate, string currency, CancellationToken cancellationToken);
}
