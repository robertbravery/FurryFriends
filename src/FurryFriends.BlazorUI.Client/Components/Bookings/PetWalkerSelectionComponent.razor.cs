using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Client.Components.Bookings;

public partial class PetWalkerSelectionComponent
{
    [Inject] public IBookingService BookingService { get; set; } = default!;
    [Inject] public ILogger<PetWalkerSelectionComponent> Logger { get; set; } = default!;

    [Parameter] public string? ServiceArea { get; set; }
    [Parameter] public Guid? SelectedPetWalkerId { get; set; }
    [Parameter] public EventCallback<PetWalkerSummaryDto> OnPetWalkerSelected { get; set; }
    [Parameter] public EventCallback<Guid?> SelectedPetWalkerIdChanged { get; set; }

    private List<PetWalkerSummaryDto> availablePetWalkers = new();
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadPetWalkersAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Reload pet walkers if service area changes
        if (ServiceArea != _previousServiceArea)
        {
            _previousServiceArea = ServiceArea;
            await LoadPetWalkersAsync();
        }
    }

    private string? _previousServiceArea;

    private async Task LoadPetWalkersAsync(string? serviceArea = null)
    {
        try
        {
            isLoading = true;
            errorMessage = null;
            StateHasChanged();

            Logger.LogInformation("Loading available pet walkers for service area: {ServiceArea}", 
                serviceArea ?? ServiceArea ?? "All");

            var response = await BookingService.GetAvailablePetWalkersAsync(serviceArea ?? ServiceArea);

            if (response.Success && response.Data != null)
            {
                availablePetWalkers = response.Data;
                Logger.LogInformation("Successfully loaded {Count} pet walkers", availablePetWalkers.Count);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to load pet walkers";
                availablePetWalkers = new List<PetWalkerSummaryDto>();
                Logger.LogWarning("Failed to load pet walkers: {Error}", errorMessage);
            }
        }
        catch (Exception ex)
        {
            errorMessage = "An error occurred while loading pet walkers";
            availablePetWalkers = new List<PetWalkerSummaryDto>();
            Logger.LogError(ex, "Error loading pet walkers");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task SelectPetWalker(PetWalkerSummaryDto petWalker)
    {
        try
        {
            Logger.LogInformation("Selecting pet walker: {PetWalkerId} - {Name}", 
                petWalker.Id, petWalker.FullName);

            SelectedPetWalkerId = petWalker.Id;
            
            // Notify parent components of the selection
            await SelectedPetWalkerIdChanged.InvokeAsync(SelectedPetWalkerId);
            await OnPetWalkerSelected.InvokeAsync(petWalker);

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error selecting pet walker: {PetWalkerId}", petWalker.Id);
        }
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength) + "...";
    }

    public async Task RefreshAsync()
    {
        await LoadPetWalkersAsync();
    }

    public void ClearSelection()
    {
        SelectedPetWalkerId = null;
        StateHasChanged();
    }
}
