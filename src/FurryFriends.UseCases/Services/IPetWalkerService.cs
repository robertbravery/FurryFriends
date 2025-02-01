using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCase.Services.DataTransferObjects;
using FurryFriends.UseCases.PetWalkers.ListPetWalker;
using FurryFriends.UseCases.Users.CreatePetWalker;

namespace FurryFriends.UseCases.Services;

public interface IPetWalkerService
{
  Task<Result<PetWalker>> CreatePetWalkerAsync(CreatePetWalkerDto dto);
  Task AddBioPictureAsync(Photo photo, Guid userId);
  Task<Result<PetWalker>> GetPetWalkerByEmailAsync(string email, CancellationToken cancellationToken);
  Task<Result<PetWalkerListDto>> ListPetWalkersAsync(ListPetWalkerQuery query);
  Task<Result<PetWalkerListDto>> ListPetWalkersByLocationAsync(ListPetWalkerByLocationQuery query);
  Task<Result> UpdatePetWalkerHourlyRateAsync(Guid userId, decimal hourlyRate, string currency, CancellationToken cancellationToken);
}
