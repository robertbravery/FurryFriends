using FurryFriends.BlazorUI.Client.Models.Timeslots;
using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Components.Pages.Timeslots;

public partial class ClientBooking
{
    [Inject] public ILogger<ClientBooking> Logger { get; set; } = default!;
    [Inject] public ITimeslotService TimeslotService { get; set; } = default!;
    [Inject] public IBookingService BookingService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    // For demo purposes - in real app, get from auth
    private Guid _clientId = Guid.NewGuid(); // This would come from authentication

    private bool isLoading = false;
    private string? errorMessage;
    private string? successMessage;

    // PetWalker selection
    private List<PetWalkerSummaryDto> availablePetWalkers = new();
    private PetWalkerSummaryDto? selectedPetWalker;

    // Date and slots
    private DateTime selectedDate = DateTime.Today;
    private List<AvailableTimeslotDto> availableSlots = new();
    private AvailableTimeslotDto? selectedSlot;

    // Custom request dialog
    private bool showCustomRequestDialog = false;
    private CustomRequestFormModel customRequestForm = new();

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("INIT: ClientBooking page initialized");
        await LoadPetWalkersAsync();
    }

    private async Task LoadPetWalkersAsync()
    {
        try
        {
            isLoading = true;
            var response = await BookingService.GetAvailablePetWalkersAsync();
            
            if (response.Success && response.Data != null)
            {
                availablePetWalkers = response.Data;
                Logger.LogInformation("Loaded {Count} pet walkers", availablePetWalkers.Count);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to load pet walkers";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading pet walkers");
            errorMessage = "An error occurred while loading pet walkers";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void SelectPetWalker(PetWalkerSummaryDto petWalker)
    {
        selectedPetWalker = petWalker;
        selectedSlot = null;
        availableSlots = new();
        Logger.LogInformation("Selected pet walker: {Name}", petWalker.FullName);
    }

    private void ClearSelection()
    {
        selectedPetWalker = null;
        selectedSlot = null;
        availableSlots = new();
    }

    private async Task LoadAvailableSlots()
    {
        if (selectedPetWalker == null || selectedDate == default) return;

        try
        {
            isLoading = true;
            var response = await TimeslotService.GetAvailableTimeslotsAsync(selectedPetWalker.Id, selectedDate);
            
            if (response.Success && response.Data != null)
            {
                availableSlots = response.Data.AvailableTimeslots;
                Logger.LogInformation("Loaded {Count} available slots", availableSlots.Count);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to load available slots";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading available slots");
            errorMessage = "An error occurred while loading available slots";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void SelectSlot(AvailableTimeslotDto slot)
    {
        selectedSlot = slot;
        Logger.LogInformation("Selected slot: {Time}", slot.StartTime);
    }

    private async Task ConfirmBooking()
    {
        if (selectedSlot == null || selectedPetWalker == null) return;

        try
        {
            isLoading = true;
            var response = await TimeslotService.BookTimeslotAsync(
                selectedSlot.TimeslotId, 
                _clientId, 
                new List<Guid>() // Would be populated from pet selection
            );
            
            if (response.Success)
            {
                successMessage = "Booking confirmed successfully!";
                Logger.LogInformation("Booking confirmed: {BookingId}", response.Data?.BookingId);
                
                // Navigate to booking management or show confirmation
                await Task.Delay(2000);
                Navigation.NavigateTo("/bookings");
            }
            else
            {
                errorMessage = response.Message ?? "Failed to confirm booking";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error confirming booking");
            errorMessage = "An error occurred while confirming booking";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowCustomRequestDialog()
    {
        customRequestForm = new CustomRequestFormModel
        {
            Date = selectedDate,
            Time = new TimeOnly(9, 0),
            Duration = 30,
            Address = string.Empty
        };
        showCustomRequestDialog = true;
    }

    private void CloseCustomRequestDialog()
    {
        showCustomRequestDialog = false;
    }

    private async Task SubmitCustomRequest()
    {
        if (selectedPetWalker == null) return;

        try
        {
            isLoading = true;
            var request = new RequestCustomTimeRequest
            {
                PetWalkerId = selectedPetWalker.Id,
                ClientId = _clientId,
                RequestedDate = DateOnly.FromDateTime(customRequestForm.Date),
                PreferredStartTime = customRequestForm.Time,
                PreferredDurationMinutes = customRequestForm.Duration,
                ClientAddress = customRequestForm.Address,
                PetIds = new List<Guid>() // Would be populated from pet selection
            };

            var response = await TimeslotService.RequestCustomTimeAsync(request);
            
            if (response.Success)
            {
                successMessage = "Custom time request submitted successfully!";
                CloseCustomRequestDialog();
                Logger.LogInformation("Custom time request submitted: {RequestId}", response.Data?.RequestId);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to submit custom time request";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting custom time request");
            errorMessage = "An error occurred while submitting custom time request";
        }
        finally
        {
            isLoading = false;
        }
    }

    private class CustomRequestFormModel
    {
        public DateTime Date { get; set; }
        public TimeOnly Time { get; set; }
        public int Duration { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
