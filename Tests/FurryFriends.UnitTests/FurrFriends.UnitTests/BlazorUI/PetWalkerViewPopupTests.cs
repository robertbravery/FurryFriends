using Bunit;
using FurryFriends.BlazorUI.Client.Models;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.Locations;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Pages.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace FurryFriends.UnitTests.BlazorUI;

public class PetWalkerViewPopupTests : TestContext
{
    public PetWalkerViewPopupTests()
    {
        Services.AddSingleton<IPetWalkerService>(new FakePetWalkerService());
        Services.AddSingleton<IScheduleService>(new FakeScheduleService());
        Services.AddSingleton<IRatingService>(new FakeRatingService());
        Services.AddSingleton<IClientContextService>(new FakeClientContextService());
        Services.AddSingleton(NullLogger<PetWalkerViewPopup>.Instance);
    }

    [Fact]
    public void RendersThreePrimaryTabs()
    {
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, "walker@example.com"));

        cut.FindAll(".tab-button").Should().HaveCount(3);
        cut.FindAll(".tab-button")[0].TextContent.Should().Contain("About");
        cut.FindAll(".tab-button")[1].TextContent.Should().Contain("Schedule & Areas");
        cut.FindAll(".tab-button")[2].TextContent.Should().Contain("Reviews");
    }

    [Fact]
    public void SwitchesToScheduleAndAreasTab()
    {
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, "walker@example.com"));

        cut.FindAll(".tab-button")[1].Click();

        cut.Find(".petwalker-content").TextContent.Should().Contain("Schedule");
    }

    private sealed class FakePetWalkerService : IPetWalkerService
    {
        public Task CreatePetWalkerAsync(PetWalkerRequestDto petWalkerModel) => Task.CompletedTask;
        public Task DeletePetWalkerAsync(string email) => Task.CompletedTask;
        public Task<PetWalkerDto> GetPetWalkerByEmailAsync(string email) => Task.FromResult(CreateWalker());
        public Task<ApiResponse<PetWalkerDetailDto>> GetPetWalkerDetailsByEmailAsync(string email) => Task.FromResult(new ApiResponse<PetWalkerDetailDto>
        {
            Success = true,
            Data = CreateDetail()
        });
        public Task<ListResponse<PetWalkerDto>> GetPetWalkersAsync(int page, int pageSize) => Task.FromResult(new ListResponse<PetWalkerDto>
        {
            RowsData = new List<PetWalkerDto> { CreateWalker() },
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 1,
            TotalPages = 1
        });
        public Task<ApiResponse<bool>> UpdatePetWalkerAsync(PetWalkerDetailDto petWalkerModel) => Task.FromResult(new ApiResponse<bool> { Success = true, Data = true });
        public Task<ApiResponse<bool>> UpdateServiceAreasAsync(Guid petWalkerId, List<ServiceAreaDto> serviceAreas) => Task.FromResult(new ApiResponse<bool> { Success = true, Data = true });
        public Task<ApiResponse<PetWalkerDetailDto>> GetPetWalkerByIdAsync(Guid petWalkerId) => Task.FromResult(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = CreateDetail() });
        public Task<ListResponse<PetWalkerDto>> GetListAsync(int page, int pageSize, string? searchTerm = null) => GetPetWalkersAsync(page, pageSize);

        private static PetWalkerDto CreateWalker() => new()
        {
            Id = Guid.Parse("929ccaf2-8c74-49bb-b9a0-ce26db0611ab"),
            Name = "Jane Walker",
            EmailAddress = "walker@example.com",
            Status = "Verified"
        };

        private static PetWalkerDetailDto CreateDetail() => new()
        {
            Id = Guid.Parse("929ccaf2-8c74-49bb-b9a0-ce26db0611ab"),
            Name = "Jane Walker",
            EmailAddress = "walker@example.com",
            Biography = "Experienced pet walker.",
            HourlyRate = 25,
            IsVerified = true,
            YearsOfExperience = 4,
            DailyPetWalkLimit = 3,
            Schedules = new List<ScheduleItemDto> { new() { DayOfWeek = DayOfWeek.Monday } }
        };
    }

    private sealed class FakeScheduleService : IScheduleService
    {
        public Task<ApiResponse<GetScheduleResponseDto>> GetScheduleAsync(Guid petWalkerId) => Task.FromResult(new ApiResponse<GetScheduleResponseDto>
        {
            Success = true,
            Data = new GetScheduleResponseDto
            {
                PetWalkerId = petWalkerId,
                Schedules = new List<ScheduleItemDto> { new() { DayOfWeek = DayOfWeek.Monday } }
            }
        });
        public Task<ApiResponse<bool>> SetScheduleAsync(Guid petWalkerId, List<ScheduleItemDto> schedules) => Task.FromResult(new ApiResponse<bool> { Success = true, Data = true });
        public Task<ApiResponse<bool>> ClearScheduleAsync(Guid petWalkerId) => Task.FromResult(new ApiResponse<bool> { Success = true, Data = true });
    }

    private sealed class FakeRatingService : IRatingService
    {
        public Task<RatingSummaryDto?> GetRatingSummaryAsync(Guid petWalkerId) => Task.FromResult<RatingSummaryDto?>(new RatingSummaryDto(petWalkerId, 5, 2, new List<RatingDto>()));
        public Task<PaginatedRatingResponse> GetRatingsAsync(Guid petWalkerId, int page = 1, int pageSize = 10) => Task.FromResult(new PaginatedRatingResponse(new List<RatingDto>(), page, pageSize, 0, 0, false, false));
        public Task<RatingResult> CreateRatingAsync(CreateRatingRequest request) => Task.FromResult(new RatingResult(true, null));
        public Task<RatingResult> UpdateRatingAsync(Guid ratingId, UpdateRatingRequest request) => Task.FromResult(new RatingResult(true, null));
        public Task<RatingResult> DeleteRatingAsync(Guid ratingId, Guid clientId) => Task.FromResult(new RatingResult(true, null));
    }

    private sealed class FakeClientContextService : IClientContextService
    {
        public Task<Guid> GetCurrentClientIdAsync() => Task.FromResult(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        public Task SetCurrentClientIdAsync(Guid clientId) => Task.CompletedTask;
    }
}
