using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Client.Components.Bookings;

public partial class ScheduleDisplayComponent
{
    [Inject] public IScheduleService ScheduleService { get; set; } = default!;
    [Inject] public IBookingService BookingService { get; set; } = default!;
    [Inject] public ILogger<ScheduleDisplayComponent> Logger { get; set; } = default!;

    [Parameter] public Guid? PetWalkerId { get; set; }
    [Parameter] public string PetWalkerName { get; set; } = "Pet Walker";
    [Parameter] public DateTime? SelectedDate { get; set; }
    [Parameter] public EventCallback<DateTime?> SelectedDateChanged { get; set; }
    [Parameter] public AvailableSlotDto? SelectedTimeSlot { get; set; }
    [Parameter] public EventCallback<AvailableSlotDto?> SelectedTimeSlotChanged { get; set; }
    [Parameter] public EventCallback<(DateTime startTime, DateTime endTime)> OnTimeSlotSelected { get; set; }

    private List<ScheduleItemDto> weeklySchedule = new();
    private List<AvailableSlotDto> availableSlots = new();
    private bool isLoading = true;
    private bool isLoadingSlots = false;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        if (PetWalkerId.HasValue)
        {
            await LoadScheduleAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (PetWalkerId.HasValue && PetWalkerId != _previousPetWalkerId)
        {
            _previousPetWalkerId = PetWalkerId;
            await LoadScheduleAsync();
        }

        if (SelectedDate.HasValue && SelectedDate != _previousSelectedDate)
        {
            _previousSelectedDate = SelectedDate;
            await LoadAvailableSlotsAsync();
        }
    }

    private Guid? _previousPetWalkerId;
    private DateTime? _previousSelectedDate;

    private async Task LoadScheduleAsync()
    {
        if (!PetWalkerId.HasValue) return;

        try
        {
            isLoading = true;
            errorMessage = null;
            StateHasChanged();

            Logger.LogInformation("Loading schedule for PetWalker: {PetWalkerId}", PetWalkerId);

            var response = await ScheduleService.GetScheduleAsync(PetWalkerId.Value);

            if (response.Success && response.Data?.Schedules != null)
            {
                weeklySchedule = response.Data.Schedules;
                Logger.LogInformation("Successfully loaded schedule with {Count} items", weeklySchedule.Count);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to load schedule";
                weeklySchedule = new List<ScheduleItemDto>();
                Logger.LogWarning("Failed to load schedule: {Error}", errorMessage);
            }
        }
        catch (Exception ex)
        {
            errorMessage = "An error occurred while loading the schedule";
            weeklySchedule = new List<ScheduleItemDto>();
            Logger.LogError(ex, "Error loading schedule for PetWalker: {PetWalkerId}", PetWalkerId);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadAvailableSlotsAsync()
    {
        if (!PetWalkerId.HasValue || !SelectedDate.HasValue) return;

        try
        {
            isLoadingSlots = true;
            StateHasChanged();

            Logger.LogInformation("Loading available slots for PetWalker: {PetWalkerId} on {Date}", 
                PetWalkerId, SelectedDate.Value.ToString("yyyy-MM-dd"));

            var response = await BookingService.GetAvailableSlotsAsync(PetWalkerId.Value, SelectedDate.Value);

            if (response.Success && response.Data?.AvailableSlots != null)
            {
                availableSlots = response.Data.AvailableSlots;
                Logger.LogInformation("Successfully loaded {Count} available slots", availableSlots.Count);
            }
            else
            {
                availableSlots = new List<AvailableSlotDto>();
                Logger.LogWarning("Failed to load available slots: {Error}", response.Message);
            }
        }
        catch (Exception ex)
        {
            availableSlots = new List<AvailableSlotDto>();
            Logger.LogError(ex, "Error loading available slots");
        }
        finally
        {
            isLoadingSlots = false;
            StateHasChanged();
        }
    }

    private async Task SelectDate(DateTime date)
    {
        try
        {
            Logger.LogInformation("Selecting date: {Date}", date.ToString("yyyy-MM-dd"));

            SelectedDate = date;
            SelectedTimeSlot = null; // Clear selected time slot when date changes
            
            await SelectedDateChanged.InvokeAsync(SelectedDate);
            await SelectedTimeSlotChanged.InvokeAsync(null);
            
            await LoadAvailableSlotsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error selecting date: {Date}", date);
        }
    }

    private async Task SelectTimeSlot(AvailableSlotDto slot)
    {
        try
        {
            Logger.LogInformation("Selecting time slot: {StartTime} - {EndTime}", 
                slot.StartTime, slot.EndTime);

            SelectedTimeSlot = slot;
            
            await SelectedTimeSlotChanged.InvokeAsync(SelectedTimeSlot);
            await OnTimeSlotSelected.InvokeAsync((slot.StartTime, slot.EndTime));

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error selecting time slot");
        }
    }

    private List<DateTime> GetWeekDays()
    {
        var startDate = SelectedDate?.Date ?? DateTime.Today;
        var startOfWeek = startDate.AddDays(-(int)startDate.DayOfWeek);
        
        return Enumerable.Range(0, 7)
            .Select(i => startOfWeek.AddDays(i))
            .ToList();
    }

    private bool IsSlotSelected(AvailableSlotDto slot)
    {
        return SelectedTimeSlot != null &&
               SelectedTimeSlot.StartTime == slot.StartTime &&
               SelectedTimeSlot.EndTime == slot.EndTime;
    }

    private int GetSlotDuration(AvailableSlotDto slot)
    {
        return (int)(slot.EndTime - slot.StartTime).TotalMinutes;
    }

    public async Task RefreshAsync()
    {
        await LoadScheduleAsync();
        if (SelectedDate.HasValue)
        {
            await LoadAvailableSlotsAsync();
        }
    }

    public void ClearSelection()
    {
        SelectedDate = null;
        SelectedTimeSlot = null;
        availableSlots.Clear();
        StateHasChanged();
    }
}
