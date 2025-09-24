using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Client.Components.Bookings;

public partial class DateTimeSelectionComponent
{
    [Inject] public IBookingService BookingService { get; set; } = default!;
    [Inject] public ILogger<DateTimeSelectionComponent> Logger { get; set; } = default!;

    [Parameter] public Guid? PetWalkerId { get; set; }
    [Parameter] public DateTime? SelectedDate { get; set; }
    [Parameter] public EventCallback<DateTime?> SelectedDateChanged { get; set; }
    [Parameter] public DateTime? SelectedStartTime { get; set; }
    [Parameter] public EventCallback<DateTime?> SelectedStartTimeChanged { get; set; }
    [Parameter] public DateTime? SelectedEndTime { get; set; }
    [Parameter] public EventCallback<DateTime?> SelectedEndTimeChanged { get; set; }
    [Parameter] public bool AllowCustomTime { get; set; } = false;
    [Parameter] public EventCallback<(DateTime startTime, DateTime endTime)> OnTimeSelectionChanged { get; set; }

    private List<AvailableSlotDto> availableSlots = new();
    private AvailableSlotDto? selectedSlot;
    private bool isLoadingSlots = false;
    private bool UseCustomTime = false;
    private TimeOnly? CustomStartTime;
    private TimeOnly? CustomEndTime;

    // Validation properties
    private bool HasDateError => !string.IsNullOrEmpty(DateErrorMessage);
    private bool HasTimeError => !string.IsNullOrEmpty(TimeErrorMessage);
    private bool HasStartTimeError => !string.IsNullOrEmpty(StartTimeErrorMessage);
    private bool HasEndTimeError => !string.IsNullOrEmpty(EndTimeErrorMessage);
    
    private string? DateErrorMessage;
    private string? TimeErrorMessage;
    private string? StartTimeErrorMessage;
    private string? EndTimeErrorMessage;

    protected override async Task OnParametersSetAsync()
    {
        if (PetWalkerId.HasValue && SelectedDate.HasValue && SelectedDate != _previousSelectedDate)
        {
            _previousSelectedDate = SelectedDate;
            await LoadAvailableSlotsAsync();
        }
    }

    private DateTime? _previousSelectedDate;

    private async Task OnDateChanged()
    {
        try
        {
            ValidateDate();
            
            if (!HasDateError && SelectedDate.HasValue)
            {
                // Clear previous selections
                selectedSlot = null;
                SelectedStartTime = null;
                SelectedEndTime = null;
                UseCustomTime = false;
                CustomStartTime = null;
                CustomEndTime = null;
                
                await SelectedDateChanged.InvokeAsync(SelectedDate);
                await SelectedStartTimeChanged.InvokeAsync(null);
                await SelectedEndTimeChanged.InvokeAsync(null);
                
                await LoadAvailableSlotsAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling date change");
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

    private async Task SelectTimeSlot(AvailableSlotDto slot)
    {
        try
        {
            Logger.LogInformation("Selecting time slot: {StartTime} - {EndTime}",
                slot.StartTime, slot.EndTime);

            selectedSlot = slot;
            UseCustomTime = false; // Clear custom time when selecting a predefined slot

            SelectedStartTime = slot.StartTime;
            SelectedEndTime = slot.EndTime;

            await SelectedStartTimeChanged.InvokeAsync(SelectedStartTime);
            await SelectedEndTimeChanged.InvokeAsync(SelectedEndTime);
            await OnTimeSelectionChanged.InvokeAsync((slot.StartTime, slot.EndTime));

            ClearTimeErrors();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error selecting time slot");
        }
    }

    private async Task ValidateCustomTime()
    {
        try
        {
            ClearTimeErrors();

            if (!CustomStartTime.HasValue || !CustomEndTime.HasValue)
                return;

            if (CustomStartTime >= CustomEndTime)
            {
                StartTimeErrorMessage = "Start time must be before end time";
                return;
            }

            var customStart = GetCustomDateTime(CustomStartTime.Value);
            var customEnd = GetCustomDateTime(CustomEndTime.Value);

            // Check if the custom time is available
            if (PetWalkerId.HasValue)
            {
                var response = await BookingService.CanBookTimeSlotAsync(PetWalkerId.Value, customStart, customEnd);

                if (!response.Success || !response.Data)
                {
                    TimeErrorMessage = "Selected time slot is not available";
                    return;
                }
            }

            // If validation passes, update the selected times
            selectedSlot = null; // Clear predefined slot selection
            SelectedStartTime = customStart;
            SelectedEndTime = customEnd;

            await SelectedStartTimeChanged.InvokeAsync(SelectedStartTime);
            await SelectedEndTimeChanged.InvokeAsync(SelectedEndTime);
            await OnTimeSelectionChanged.InvokeAsync((customStart, customEnd));

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating custom time");
            TimeErrorMessage = "Error validating selected time";
        }
    }

    private void ValidateDate()
    {
        DateErrorMessage = null;

        if (!SelectedDate.HasValue)
        {
            DateErrorMessage = "Please select a date";
            return;
        }

        if (SelectedDate.Value.Date < DateTime.Today)
        {
            DateErrorMessage = "Cannot select a date in the past";
            return;
        }

        if (SelectedDate.Value.Date > DateTime.Today.AddMonths(3))
        {
            DateErrorMessage = "Cannot book more than 3 months in advance";
            return;
        }
    }

    private void ClearTimeErrors()
    {
        TimeErrorMessage = null;
        StartTimeErrorMessage = null;
        EndTimeErrorMessage = null;
    }

    // Helper methods
    private bool IsSlotSelected(AvailableSlotDto slot)
    {
        return selectedSlot != null &&
               selectedSlot.StartTime == slot.StartTime &&
               selectedSlot.EndTime == slot.EndTime;
    }

    private int GetSlotDuration(AvailableSlotDto slot)
    {
        return (int)(slot.EndTime - slot.StartTime).TotalMinutes;
    }

    private DateTime GetCustomDateTime(TimeOnly time)
    {
        return SelectedDate!.Value.Date.Add(time.ToTimeSpan());
    }

    private int GetCustomDuration()
    {
        if (!CustomStartTime.HasValue || !CustomEndTime.HasValue)
            return 0;

        return (int)(CustomEndTime.Value.ToTimeSpan() - CustomStartTime.Value.ToTimeSpan()).TotalMinutes;
    }

    private bool HasValidSelection()
    {
        return SelectedDate.HasValue && SelectedStartTime.HasValue && SelectedEndTime.HasValue;
    }

    private string GetSelectedTimeRange()
    {
        if (!SelectedStartTime.HasValue || !SelectedEndTime.HasValue)
            return "Not selected";

        return $"{SelectedStartTime.Value:HH:mm} - {SelectedEndTime.Value:HH:mm}";
    }

    private int GetSelectedDuration()
    {
        if (!SelectedStartTime.HasValue || !SelectedEndTime.HasValue)
            return 0;

        return (int)(SelectedEndTime.Value - SelectedStartTime.Value).TotalMinutes;
    }

    private decimal GetSelectedPrice()
    {
        if (selectedSlot != null)
            return selectedSlot.Price;

        // For custom time, you might calculate price based on duration and hourly rate
        return 0;
    }

    public void ClearSelection()
    {
        SelectedDate = null;
        SelectedStartTime = null;
        SelectedEndTime = null;
        selectedSlot = null;
        UseCustomTime = false;
        CustomStartTime = null;
        CustomEndTime = null;
        availableSlots.Clear();
        ClearTimeErrors();
        DateErrorMessage = null;
        StateHasChanged();
    }

    public bool IsValid()
    {
        ValidateDate();
        return HasValidSelection() && !HasDateError && !HasTimeError && !HasStartTimeError && !HasEndTimeError;
    }
}
