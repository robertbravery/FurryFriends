using FurryFriends.BlazorUI.Client.Models;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

/// <summary>
/// Service for managing pet walkers
/// </summary>
public interface IPetWalkerService : IListService<PetWalkerDto>
{
  /// <summary>
  /// Creates a new pet walker
  /// </summary>
  Task CreatePetWalkerAsync(PetWalkerRequestDto petWalkerModel);

  /// <summary>
  /// Deletes a pet walker by email
  /// </summary>
  Task DeletePetWalkerAsync(string email);

  /// <summary>
  /// Gets a pet walker by email
  /// </summary>
  Task<PetWalkerDto> GetPetWalkerByEmailAsync(string email);

  /// <summary>
  /// Gets detailed information about a pet walker by email
  /// </summary>
  Task<ApiResponse<PetWalkerDetailDto>> GetPetWalkerDetailsByEmailAsync(string email);

  /// <summary>
  /// Gets a paginated list of pet walkers
  /// </summary>
  Task<ListResponse<PetWalkerDto>> GetPetWalkersAsync(int page, int pageSize);

  /// <summary>
  /// Updates an existing pet walker
  /// </summary>
  Task<ApiResponse<bool>> UpdatePetWalkerAsync(PetWalkerDetailDto petWalkerModel);
}
