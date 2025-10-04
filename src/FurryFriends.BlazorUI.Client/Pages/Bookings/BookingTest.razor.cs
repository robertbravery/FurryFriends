using FurryFriends.BlazorUI.Client.Models.Bookings;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Client.Pages.Bookings;

public partial class BookingTest
{
    [Inject] public ILogger<BookingTest> Logger { get; set; } = default!;

    private string selectedComponent = "petwalker";
    private string? testServiceArea = null; // Can be set to test filtering
    
    // Selection state
    private Guid? selectedPetWalkerId;
    private PetWalkerSummaryDto? selectedPetWalker;
    private DateTime? selectedDate;
    private DateTime? selectedStartTime;
    private DateTime? selectedEndTime;

    protected override void OnInitialized()
    {
        Logger.LogInformation("BookingTest page initialized");
    }

    private void SelectComponent(string component)
    {
        selectedComponent = component;
        Logger.LogInformation("Selected component: {Component}", component);
        StateHasChanged();
    }

    private Task OnPetWalkerSelected(PetWalkerSummaryDto petWalker)
    {
        try
        {
            selectedPetWalker = petWalker;
            Logger.LogInformation("PetWalker selected: {PetWalkerId} - {Name}", 
                petWalker.Id, petWalker.FullName);
            
            StateHasChanged();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling PetWalker selection");
            return Task.CompletedTask;
        }
    }

    private Task OnPetWalkerIdChanged(Guid? petWalkerId)
    {
        try
        {
            selectedPetWalkerId = petWalkerId;
            Logger.LogInformation("PetWalker ID changed: {PetWalkerId}", petWalkerId);
            
            // Clear date/time selections when PetWalker changes
            selectedDate = null;
            selectedStartTime = null;
            selectedEndTime = null;
            
            StateHasChanged();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling PetWalker ID change");
            return Task.CompletedTask;
        }
    }

    private Task OnDateChanged(DateTime? date)
    {
        try
        {
            selectedDate = date;
            Logger.LogInformation("Date changed: {Date}", date?.ToString("yyyy-MM-dd"));
            
            // Clear time selections when date changes
            selectedStartTime = null;
            selectedEndTime = null;
            
            StateHasChanged();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling date change");
            return Task.CompletedTask;
        }
    }

    private Task OnStartTimeChanged(DateTime? startTime)
    {
        try
        {
            selectedStartTime = startTime;
            Logger.LogInformation("Start time changed: {StartTime}", startTime?.ToString("HH:mm"));
            StateHasChanged();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling start time change");
            return Task.CompletedTask;
        }
    }

    private Task OnEndTimeChanged(DateTime? endTime)
    {
        try
        {
            selectedEndTime = endTime;
            Logger.LogInformation("End time changed: {EndTime}", endTime?.ToString("HH:mm"));
            StateHasChanged();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling end time change");
            return Task.CompletedTask;
        }
    }

    private Task OnTimeSlotSelected((DateTime startTime, DateTime endTime) timeSlot)
    {
        try
        {
            selectedStartTime = timeSlot.startTime;
            selectedEndTime = timeSlot.endTime;
            Logger.LogInformation("Time slot selected: {StartTime} - {EndTime}", 
                timeSlot.startTime.ToString("HH:mm"), timeSlot.endTime.ToString("HH:mm"));
            StateHasChanged();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling time slot selection");
            return Task.CompletedTask;
        }
    }

    private Task OnTimeSelectionChanged((DateTime startTime, DateTime endTime) timeSelection)
    {
        try
        {
            selectedStartTime = timeSelection.startTime;
            selectedEndTime = timeSelection.endTime;
            Logger.LogInformation("Time selection changed: {StartTime} - {EndTime}", 
                timeSelection.startTime.ToString("HH:mm"), timeSelection.endTime.ToString("HH:mm"));
            StateHasChanged();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling time selection change");
            return Task.CompletedTask;
        }
    }
}
