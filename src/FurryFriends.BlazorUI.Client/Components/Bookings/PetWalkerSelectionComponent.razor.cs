using System.Collections.Concurrent;
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

    /// <summary>
    /// Optional date to show per-walker availability for. When set, each card reports how
    /// many slots the walker has free, so availability is visible before choosing a walker.
    /// </summary>
    [Parameter] public DateTime? SelectedDate { get; set; }
    [Parameter] public EventCallback<DateTime?> SelectedDateChanged { get; set; }

    private List<PetWalkerSummaryDto> availablePetWalkers = new();
    private bool isLoading = true;
    private bool isLoadingAvailability;
    private string? errorMessage;
    private DateTime? _loadedAvailabilityDate;

    /// <summary>Per-walker availability for the currently selected date.</summary>
    private readonly ConcurrentDictionary<Guid, WalkerAvailability> _availability = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadPetWalkersAsync();
        await LoadAvailabilityAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Reload pet walkers if service area changes
        if (ServiceArea != _previousServiceArea)
        {
            _previousServiceArea = ServiceArea;
            await LoadPetWalkersAsync();
            // _loadedAvailabilityDate is unchanged, so force a re-probe for the new walker set.
            _loadedAvailabilityDate = null;
            await LoadAvailabilityAsync();
        }

        // Reload availability when the date changes, including the first time one is supplied.
        if (SelectedDate?.Date != _loadedAvailabilityDate?.Date)
        {
            await LoadAvailabilityAsync();
        }
    }

    private string? _previousServiceArea;

    /// <summary>
    /// Probes each walker's availability for the selected date. Skips the lookup entirely when
    /// no date is chosen, so the default step-1 experience costs no extra calls.
    /// </summary>
    private async Task LoadAvailabilityAsync()
    {
        _availability.Clear();
        _loadedAvailabilityDate = SelectedDate?.Date;

        if (!SelectedDate.HasValue)
        {
            return;
        }

        isLoadingAvailability = true;
        StateHasChanged();

        try
        {
            var tasks = availablePetWalkers.Select(async walker =>
            {
                try
                {
                    var response = await BookingService.GetAvailableSlotsAsync(walker.Id, SelectedDate.Value);

                    _availability[walker.Id] = response.Success && response.Data != null
                        ? new WalkerAvailability(true, response.Data.HasSchedule, response.Data.AvailableSlots.Count)
                        : new WalkerAvailability(false, false, 0);
                }
                catch (Exception ex)
                {
                    // One walker failing must not break the whole list.
                    Logger.LogWarning(ex, "Failed to read availability for PetWalker {PetWalkerId}", walker.Id);
                    _availability[walker.Id] = new WalkerAvailability(false, false, 0);
                }
            });

            await Task.WhenAll(tasks);

            Logger.LogInformation(
                "Loaded availability for {Total} walkers on {Date:yyyy-MM-dd}: {Available} have slots",
                availablePetWalkers.Count, SelectedDate.Value,
                _availability.Values.Count(a => a.IsAvailable && a.SlotCount > 0));
        }
        finally
        {
            isLoadingAvailability = false;
            StateHasChanged();
        }
    }

    private async Task OnDateChangedAsync(ChangeEventArgs args)
    {
        SelectedDate = DateTime.TryParse(args.Value?.ToString(), out var date) ? date : null;

        // Keep the parent in step so the date is not lost when advancing to the time step.
        await SelectedDateChanged.InvokeAsync(SelectedDate);
        await LoadAvailabilityAsync();
    }

    private WalkerAvailability AvailabilityFor(Guid petWalkerId) =>
        _availability.TryGetValue(petWalkerId, out var availability)
            ? availability
            : new WalkerAvailability(false, false, 0);

    /// <summary>A walker with no free slots can still be inspected, but not booked.</summary>
    private static bool CanBeSelected(WalkerAvailability availability) =>
        !availability.IsAvailable || availability.SlotCount > 0;

    private sealed record WalkerAvailability(bool IsAvailable, bool HasSchedule, int SlotCount);

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
            if (!CanBeSelected(AvailabilityFor(petWalker.Id)))
            {
                Logger.LogInformation(
                    "Ignored selection of PetWalker {PetWalkerId}: no free slots on {Date:yyyy-MM-dd}",
                    petWalker.Id, SelectedDate);
                return;
            }

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
