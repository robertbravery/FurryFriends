using FurryFriends.BlazorUI.Client.Models.Timeslots;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Components.Pages.Timeslots;

public partial class PetwalkerAvailability
{
    [Inject] public ILogger<PetwalkerAvailability> Logger { get; set; } = default!;
    [Inject] public ITimeslotService TimeslotService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    // For demo purposes - in real app, get from auth
    private Guid _petWalkerId = Guid.NewGuid(); // This would come from authentication

    private bool isLoading = false;
    private string? errorMessage;
    private string? successMessage;
    private string activeTab = "hours";

    // Working Hours
    private List<WorkingHoursDto> workingHoursList = new();
    private bool showWorkingHoursDialog = false;
    private WorkingHoursDto? editingWorkingHours;
    private WorkingHoursFormModel workingHoursForm = new();

    // Timeslots
    private List<TimeslotDto> timeslotsList = new();
    private bool showTimeslotDialog = false;
    private DateOnly? filterStartDate;
    private DateOnly? filterEndDate;
    private TimeslotFormModel timeslotForm = new();

    // Custom Requests
    private List<CustomTimeRequestDto> customRequestsList = new();
    private string? requestStatusFilter;
    private bool showCounterOfferDialog = false;
    private CustomTimeRequestDto? selectedRequest;
    private CounterOfferFormModel counterOfferForm = new();

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("INIT: PetwalkerAvailability page initialized");
        await LoadWorkingHoursAsync();
    }

    #region Tab Navigation

    private void SetHoursTab()
    {
        activeTab = "hours";
        _ = LoadWorkingHoursAsync();
    }

    private void SetSlotsTab()
    {
        activeTab = "slots";
        _ = LoadTimeslots();
    }

    private void SetRequestsTab()
    {
        activeTab = "requests";
        _ = LoadCustomRequests();
    }

    #endregion

    #region Working Hours

    private async Task LoadWorkingHoursAsync()
    {
        try
        {
            isLoading = true;
            var response = await TimeslotService.GetWorkingHoursAsync(_petWalkerId);
            
            if (response.Success && response.Data != null)
            {
                workingHoursList = response.Data.WorkingHours;
                Logger.LogInformation("Loaded {Count} working hours", workingHoursList.Count);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to load working hours";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading working hours");
            errorMessage = "An error occurred while loading working hours";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowAddWorkingHoursDialog()
    {
        editingWorkingHours = null;
        workingHoursForm = new WorkingHoursFormModel
        {
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        showWorkingHoursDialog = true;
    }

    private void EditWorkingHours(WorkingHoursDto hours)
    {
        editingWorkingHours = hours;
        workingHoursForm = new WorkingHoursFormModel
        {
            DayOfWeek = hours.DayOfWeek,
            StartTime = hours.StartTime,
            EndTime = hours.EndTime,
            IsActive = hours.IsActive
        };
        showWorkingHoursDialog = true;
    }

    private void CloseWorkingHoursDialog()
    {
        showWorkingHoursDialog = false;
        editingWorkingHours = null;
    }

    private async Task SaveWorkingHours()
    {
        try
        {
            if (editingWorkingHours != null)
            {
                // Update existing
                var response = await TimeslotService.UpdateWorkingHoursAsync(new UpdateWorkingHoursRequest
                {
                    WorkingHoursId = editingWorkingHours.WorkingHoursId,
                    StartTime = workingHoursForm.StartTime,
                    EndTime = workingHoursForm.EndTime,
                    IsActive = workingHoursForm.IsActive
                });

                if (response.Success)
                {
                    successMessage = "Working hours updated successfully";
                    await LoadWorkingHoursAsync();
                }
                else
                {
                    errorMessage = response.Message ?? "Failed to update working hours";
                }
            }
            else
            {
                // Create new
                var response = await TimeslotService.CreateWorkingHoursAsync(new CreateWorkingHoursRequest
                {
                    PetWalkerId = _petWalkerId,
                    DayOfWeek = workingHoursForm.DayOfWeek,
                    StartTime = workingHoursForm.StartTime,
                    EndTime = workingHoursForm.EndTime
                });

                if (response.Success)
                {
                    successMessage = "Working hours created successfully";
                    await LoadWorkingHoursAsync();
                }
                else
                {
                    errorMessage = response.Message ?? "Failed to create working hours";
                }
            }
            CloseWorkingHoursDialog();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving working hours");
            errorMessage = "An error occurred while saving working hours";
        }
    }

    private async Task DeleteWorkingHours(Guid workingHoursId)
    {
        try
        {
            var response = await TimeslotService.DeleteWorkingHoursAsync(workingHoursId);
            
            if (response.Success)
            {
                successMessage = "Working hours deleted successfully";
                await LoadWorkingHoursAsync();
            }
            else
            {
                errorMessage = response.Message ?? "Failed to delete working hours";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting working hours");
            errorMessage = "An error occurred while deleting working hours";
        }
    }

    #endregion

    #region Timeslots

    private async Task LoadTimeslots()
    {
        try
        {
            isLoading = true;
            var response = await TimeslotService.GetTimeslotsAsync(new GetTimeslotsRequest
            {
                PetWalkerId = _petWalkerId,
                StartDate = filterStartDate,
                EndDate = filterEndDate
            });
            
            if (response.Success && response.Data != null)
            {
                timeslotsList = response.Data;
                Logger.LogInformation("Loaded {Count} timeslots", timeslotsList.Count);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to load timeslots";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading timeslots");
            errorMessage = "An error occurred while loading timeslots";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowAddTimeslotDialog()
    {
        timeslotForm = new TimeslotFormModel
        {
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime = new TimeOnly(9, 0),
            DurationInMinutes = 30
        };
        showTimeslotDialog = true;
    }

    private void CloseTimeslotDialog()
    {
        showTimeslotDialog = false;
    }

    private async Task SaveTimeslot()
    {
        try
        {
            var response = await TimeslotService.CreateTimeslotAsync(new CreateTimeslotRequest
            {
                PetWalkerId = _petWalkerId,
                Date = timeslotForm.Date,
                StartTime = timeslotForm.StartTime,
                DurationInMinutes = timeslotForm.DurationInMinutes
            });

            if (response.Success)
            {
                successMessage = "Timeslot created successfully";
                CloseTimeslotDialog();
                await LoadTimeslots();
            }
            else
            {
                errorMessage = response.Message ?? "Failed to create timeslot";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating timeslot");
            errorMessage = "An error occurred while creating timeslot";
        }
    }

    private async Task DeleteTimeslot(Guid timeslotId)
    {
        try
        {
            var response = await TimeslotService.DeleteTimeslotAsync(timeslotId);
            
            if (response.Success)
            {
                successMessage = "Timeslot deleted successfully";
                await LoadTimeslots();
            }
            else
            {
                errorMessage = response.Message ?? "Failed to delete timeslot";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting timeslot");
            errorMessage = "An error occurred while deleting timeslot";
        }
    }

    #endregion

    #region Custom Requests

    private async Task LoadCustomRequests()
    {
        try
        {
            isLoading = true;
            var response = await TimeslotService.GetPetWalkerCustomTimeRequestsAsync(_petWalkerId, requestStatusFilter);
            
            if (response.Success && response.Data != null)
            {
                customRequestsList = response.Data;
                Logger.LogInformation("Loaded {Count} custom requests", customRequestsList.Count);
            }
            else
            {
                errorMessage = response.Message ?? "Failed to load custom requests";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading custom requests");
            errorMessage = "An error occurred while loading custom requests";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task OnStatusFilterChanged(ChangeEventArgs e)
    {
        requestStatusFilter = e.Value?.ToString();
        await LoadCustomRequests();
    }

    private async Task AcceptRequest(Guid requestId)
    {
        try
        {
            var response = await TimeslotService.RespondToCustomTimeRequestAsync(new RespondToCustomTimeRequestRequest
            {
                RequestId = requestId,
                Action = "Accept"
            });

            if (response.Success)
            {
                successMessage = "Request accepted successfully";
                await LoadCustomRequests();
            }
            else
            {
                errorMessage = response.Message ?? "Failed to accept request";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error accepting request");
            errorMessage = "An error occurred while accepting request";
        }
    }

    private async Task DeclineRequest(Guid requestId)
    {
        try
        {
            var response = await TimeslotService.RespondToCustomTimeRequestAsync(new RespondToCustomTimeRequestRequest
            {
                RequestId = requestId,
                Action = "Decline"
            });

            if (response.Success)
            {
                successMessage = "Request declined";
                await LoadCustomRequests();
            }
            else
            {
                errorMessage = response.Message ?? "Failed to decline request";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error declining request");
            errorMessage = "An error occurred while declining request";
        }
    }

    private void ShowCounterOfferDialog(CustomTimeRequestDto request)
    {
        selectedRequest = request;
        counterOfferForm = new CounterOfferFormModel
        {
            Date = request.RequestedDate,
            StartTime = request.PreferredStartTime,
            DurationMinutes = request.PreferredDurationMinutes
        };
        showCounterOfferDialog = true;
    }

    private void CloseCounterOfferDialog()
    {
        showCounterOfferDialog = false;
        selectedRequest = null;
    }

    private async Task SubmitCounterOffer()
    {
        if (selectedRequest == null) return;

        try
        {
            var response = await TimeslotService.RespondToCustomTimeRequestAsync(new RespondToCustomTimeRequestRequest
            {
                RequestId = selectedRequest.RequestId,
                Action = "CounterOffer",
                CounterOfferedDate = counterOfferForm.Date,
                CounterOfferedStartTime = counterOfferForm.StartTime,
                CounterOfferedDurationMinutes = counterOfferForm.DurationMinutes
            });

            if (response.Success)
            {
                successMessage = "Counter offer sent successfully";
                CloseCounterOfferDialog();
                await LoadCustomRequests();
            }
            else
            {
                errorMessage = response.Message ?? "Failed to send counter offer";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error sending counter offer");
            errorMessage = "An error occurred while sending counter offer";
        }
    }

    #endregion

    #region Form Models

    private class WorkingHoursFormModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }

    private class TimeslotFormModel
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int DurationInMinutes { get; set; }
    }

    private class CounterOfferFormModel
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int DurationMinutes { get; set; }
    }

    #endregion
}
