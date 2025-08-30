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
    public IJSRuntime JS { get; set; } = default!;

    [Inject]
    public ILogger<PetWalkerViewPopup> Logger { get; set; } = default!;

    private PetWalkerDetailDto petWalkerModel = new();
    private bool isLoading = true;
    private string? loadError = null;

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

                // Load schedule data
                await LoadScheduleData(petWalkerModel.Id);
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
