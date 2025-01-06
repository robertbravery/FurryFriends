using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Services.DataTransferObjects;
using FurryFriends.UseCases.Users.ListUser;

namespace FurryFriends.UseCases.Services;

public interface IPetWalkerService
{
  Task<PetWalker> CreatePetWalkerAsync(PetWalker user);
  Task AddBioPictureAsync(Photo photo, Guid userId);
  Task<Result<PetWalker>> GetPetWalkerByEmailAsync(string email, CancellationToken cancellationToken);
  Task<Result<PetWalkerListDto>> ListPetWalkersAsync(ListPetWalkerQuery query);
  Task<Result<PetWalkerListDto>> ListPetWalkersByLocationAsync(ListPetWalkerByLocationQuery query);
  Task<Result> UpdatePetWalkerHourlyRateAsync(Guid userId, decimal hourlyRate, string currency, CancellationToken cancellationToken);
}
