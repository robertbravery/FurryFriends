using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace FurryFriends.BlazorUI.Client.Pages.PetWalkers;

public partial class PetWalkerViewPopup
{
    [Parameter]
    public string PetWalkerEmail { get; set; } = default!;

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Inject]
    public IPetWalkerService PetWalkerService { get; set; } = default!;

    [Inject]
    public IScheduleService ScheduleService { get; set; } = default!;

    [Inject]
    public IRatingService RatingService { get; set; } = default!;

    [Inject]
    public IClientContextService ClientContextService { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [Inject]
    public ILogger<PetWalkerViewPopup> Logger { get; set; } = default!;

    private PetWalkerDetailDto petWalkerModel = new();
    private bool isLoading = true;
    private string? loadError = null;
    private RatingSummaryDto? ratingSummary;

    // Client ID resolved from logged-in user context
    private Guid _clientId;

    // Rating summary display
    public double AverageRating => ratingSummary?.AverageRating ?? 0;
    public int TotalRatings => ratingSummary?.TotalRatings ?? 0;

    // Ratings list state
    private List<RatingDto> _ratings = new();
    private int _ratingsCurrentPage = 1;
    private const int RatingsPageSize = 10;
    private int _ratingsTotalCount;
    private int _ratingsTotalPages;
    private bool _hasPreviousPage;
    private bool _hasNextPage;
    private bool _isLoadingRatings;
    private string? _ratingsError;

    protected override async Task OnInitializedAsync()
    {
        await LoadPetWalkerData();
    }

    private async Task LoadPetWalkerData()
    {
        try
        {
            Logger.LogInformation("Loading pet walker data for email: {PetWalkerEmail}", PetWalkerEmail);
            isLoading = true;
            StateHasChanged();

            var response = await PetWalkerService.GetPetWalkerDetailsByEmailAsync(PetWalkerEmail);

            if (response != null && response.Success && response.Data != null)
            {
                Logger.LogInformation("Successfully loaded pet walker data for email: {PetWalkerEmail}", PetWalkerEmail);
                petWalkerModel = response.Data;
                loadError = null;

                // Resolve client ID from logged-in user context
                _clientId = await ClientContextService.GetCurrentClientIdAsync();
                Logger.LogInformation("Resolved client ID: {ClientId} for PetWalker view", _clientId);

                // Load schedule data
                await LoadScheduleData(petWalkerModel.Id);

                // Load rating summary
                await LoadRatingSummary();

                // Load paginated ratings list
                await LoadRatingsAsync();
            }
            else
            {
                var errorMessage = response?.Message ?? "Failed to load pet walker data.";
                var errors = response?.Errors != null && response.Errors.Any()
                    ? string.Join(", ", response.Errors)
                    : string.Empty;

                loadError = !string.IsNullOrEmpty(errors)
                    ? $"{errorMessage}: {errors}"
                    : errorMessage;

                Logger.LogWarning("Failed to load pet walker data. Email: {PetWalkerEmail}, Error: {ErrorMessage}, Details: {ErrorDetails}",
                    PetWalkerEmail, errorMessage, errors);
            }
        }
        catch (Exception ex)
        {
            loadError = $"Error loading pet walker: {ex.Message}";
            Logger.LogError(ex, "Error loading pet walker data for email: {PetWalkerEmail}", PetWalkerEmail);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadScheduleData(Guid petWalkerId)
    {
        try
        {
            Logger.LogInformation("Loading schedule data for PetWalker: {PetWalkerId}", petWalkerId);

            var scheduleResponse = await ScheduleService.GetScheduleAsync(petWalkerId);

            if (scheduleResponse.Success && scheduleResponse.Data != null)
            {
                petWalkerModel.Schedules = scheduleResponse.Data.Schedules;
                Logger.LogInformation("Successfully loaded schedule with {Count} items", petWalkerModel.Schedules.Count);
            }
            else
            {
                Logger.LogWarning("Failed to load schedule: {Error}", scheduleResponse.Message);
                petWalkerModel.Schedules = new List<ScheduleItemDto>();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading schedule data for PetWalker: {PetWalkerId}", petWalkerId);
            petWalkerModel.Schedules = new List<ScheduleItemDto>();
        }
    }

    private async Task LoadRatingSummary()
    {
        try
        {
            Logger.LogInformation("Loading rating summary for PetWalker: {PetWalkerId}", petWalkerModel.Id);
            ratingSummary = await RatingService.GetRatingSummaryAsync(petWalkerModel.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading rating summary for PetWalker: {PetWalkerId}", petWalkerModel.Id);
            ratingSummary = null;
        }
    }

    public async Task HandleRatingSubmitted()
    {
        Logger.LogInformation("Rating submitted for PetWalker: {PetWalkerId}, refreshing summary and list", petWalkerModel.Id);
        await LoadRatingSummary();
        _ratingsCurrentPage = 1;
        await LoadRatingsAsync();
        StateHasChanged();
    }

    // --- Ratings list loading & pagination ---

    public async Task LoadRatingsAsync()
    {
        if (petWalkerModel.Id == Guid.Empty) return;

        try
        {
            _isLoadingRatings = true;
            _ratingsError = null;
            StateHasChanged();

            Logger.LogInformation(
                "Loading ratings list for PetWalker: {PetWalkerId}, Page: {Page}",
                petWalkerModel.Id, _ratingsCurrentPage);

            var result = await RatingService.GetRatingsAsync(petWalkerModel.Id, _ratingsCurrentPage, RatingsPageSize);

            _ratings = result.Items;
            _ratingsCurrentPage = result.PageNumber;
            _ratingsTotalCount = result.TotalCount;
            _ratingsTotalPages = result.TotalPages;
            _hasPreviousPage = result.HasPreviousPage;
            _hasNextPage = result.HasNextPage;

            Logger.LogInformation(
                "Loaded {Count} ratings for PetWalker: {PetWalkerId} (Page {Page}/{TotalPages})",
                _ratings.Count, petWalkerModel.Id, _ratingsCurrentPage, _ratingsTotalPages);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading ratings for PetWalker: {PetWalkerId}", petWalkerModel.Id);
            _ratingsError = "Failed to load ratings. Please try again later.";
            _ratings = new List<RatingDto>();
        }
        finally
        {
            _isLoadingRatings = false;
            StateHasChanged();
        }
    }

    public async Task GoToPreviousPage()
    {
        if (!_hasPreviousPage) return;
        _ratingsCurrentPage--;
        await LoadRatingsAsync();
    }

    public async Task GoToNextPage()
    {
        if (!_hasNextPage) return;
        _ratingsCurrentPage++;
        await LoadRatingsAsync();
    }

    private string FormatPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return string.Empty;

        // Format the phone number as needed
        return phoneNumber.Length == 10
            ? $"({phoneNumber.Substring(0, 3)}) {phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6)}"
            : phoneNumber;
    }
}
