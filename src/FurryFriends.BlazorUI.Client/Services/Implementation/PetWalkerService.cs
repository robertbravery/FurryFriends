//using FurryFriends.BlazorUI.Client.Models;
//using FurryFriends.BlazorUI.Client.Models.Common;
//using FurryFriends.BlazorUI.Client.Models.PetWalkers;
//using FurryFriends.BlazorUI.Client.Services.Interfaces;
//using System.Net.Http.Json;

//namespace FurryFriends.BlazorUI.Client.Services.Implementation;

//public class PetWalkerService : BaseListService<PetWalkerDto>, IPetWalkerService
//{
//    private readonly HttpClient _httpClient;
//    private readonly string _apiBaseUrl;

//    public PetWalkerService(HttpClient httpClient, IConfiguration configuration)
//        : base(httpClient, configuration, "PetWalker/list")
//    {
//        _httpClient = httpClient;
//        _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
//    }

//    public async Task<ListResponse<PetWalkerDto>> GetPetWalkersAsync(int page, int pageSize)
//    {
//        return await GetListAsync(page, pageSize);
//    }

//    public async Task<PetWalkerDto> GetPetWalkerByEmailAsync(string email)
//    {
//        try
//        {
//            var response = await _httpClient.GetFromJsonAsync<PetWalkerDto>($"{_apiBaseUrl}/PetWalkers/{email}");
//            return response ?? new PetWalkerDto();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error getting pet walker by email: {ex.Message}");
//            return new PetWalkerDto();
//        }
//    }

//    public async Task<ResponseBase<PetWalkerDetailDto>> GetPetWalkerDetailsByEmailAsync(string email)
//    {
//        try
//        {
//            // In a real implementation, we would call the API
//            // var response = await _httpClient.GetFromJsonAsync<ResponseBase<PetWalkerDetailDto>>($"{_apiBaseUrl}/PetWalkers/details/{email}");

//            // For now, we'll return mock data
//            await Task.Delay(500); // Simulate network delay

//            var mockPetWalker = new PetWalkerDetailDto
//            {
//                Id = Guid.NewGuid(),
//                Name = "John Walker",
//                EmailAddress = email,
//                PhoneNumber = "5551234567",
//                Street = "123 Main St",
//                City = "Dogtown",
//                State = "CA",
//                ZipCode = "90210",
//                Country = "USA",
//                Biography = "I've been a professional pet walker for 5 years. I love all animals, especially dogs and cats. I'm reliable, punctual, and will treat your pets like my own.",
//                DateOfBirth = new DateTime(1985, 6, 15),
//                Gender = "Male",
//                HourlyRate = 25.00m,
//                Currency = "USD",
//                IsActive = true,
//                IsVerified = true,
//                YearsOfExperience = 5,
//                HasInsurance = true,
//                HasFirstAidCertification = true,
//                DailyPetWalkLimit = 8,
//                ServiceAreas = new List<string> { "Dogtown", "Catville", "Petsburg" },
//                ProfilePicture = new PhotoDto
//                {
//                    Url = "https://randomuser.me/api/portraits/men/32.jpg",
//                    Description = "Profile photo"
//                },
//                Photos = new List<PhotoDto>
//                {
//                    new PhotoDto
//                    {
//                        Url = "https://images.unsplash.com/photo-1548199973-03cce0bbc87b",
//                        Description = "Walking dogs in the park"
//                    },
//                    new PhotoDto
//                    {
//                        Url = "https://images.unsplash.com/photo-1535930891776-0c2dfb7fda1a",
//                        Description = "With a client's cat"
//                    },
//                    new PhotoDto
//                    {
//                        Url = "https://images.unsplash.com/photo-1576201836106-db1758fd1c97",
//                        Description = "Pet first aid training"
//                    }
//                }
//            };

//            return new ResponseBase<PetWalkerDetailDto>(mockPetWalker, true, "Success");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error getting pet walker details: {ex.Message}");
//            return new ResponseBase<PetWalkerDetailDto>(null, false, "Failed to load pet walker details", new List<string> { ex.Message });
//        }
//    }

//    public async Task CreatePetWalkerAsync(PetWalkerRequestDto petWalkerModel)
//    {
//        await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/PetWalkers", petWalkerModel);
//    }

//    public async Task UpdatePetWalkerAsync(PetWalkerRequestDto petWalkerModel)
//    {
//        await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/PetWalkers/{petWalkerModel.Id}", petWalkerModel);
//    }

//    public async Task DeletePetWalkerAsync(string email)
//    {
//        await _httpClient.DeleteAsync($"{_apiBaseUrl}/PetWalkers/{email}");
//    }
//}
