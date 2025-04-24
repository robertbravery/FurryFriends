using FurryFriends.BlazorUI.Client.Models;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class PetWalkerService : BaseListService<PetWalkerDto>, IPetWalkerService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;

  public PetWalkerService(HttpClient httpClient, IConfiguration configuration)
    : base(httpClient, configuration, "PetWalker/list")
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
  }

  public async Task<ListResponse<PetWalkerDto>> GetPetWalkersAsync(int page, int pageSize)
  {
    return await GetListAsync(page, pageSize);
  }

  public async Task<PetWalkerDto> GetPetWalkerByEmailAsync(string email)
  {
    var response = await _httpClient.GetFromJsonAsync<PetWalkerDto>($"{_apiBaseUrl}/PetWalkers/{email}");
    return response ?? new PetWalkerDto();
  }

  public async Task CreatePetWalkerAsync(PetWalkerRequestDto petWalkerModel)
  {
    await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/PetWalkers", petWalkerModel);
  }

  public async Task UpdatePetWalkerAsync(PetWalkerRequestDto petWalkerModel)
  {
    await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/PetWalkers/{petWalkerModel.Id}", petWalkerModel);
  }

  public async Task DeletePetWalkerAsync(string email)
  {
    await _httpClient.DeleteAsync($"{_apiBaseUrl}/PetWalkers/{email}");
  }
}
