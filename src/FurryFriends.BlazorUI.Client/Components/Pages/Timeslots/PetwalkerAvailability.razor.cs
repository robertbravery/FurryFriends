using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Models.Timeslots;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Components.Pages.Timeslots;

public partial class PetwalkerAvailability
{
  [Inject] public ILogger<PetwalkerAvailability> Logger { get; set; } = default!;
  [Inject] public IScheduleService ScheduleService { get; set; } = default!;
  [Inject] public IPetWalkerService PetWalkerService { get; set; } = default!;
  [Inject] public ITimeslotService TimeslotService { get; set; } = default!;
  [Inject] public NavigationManager Navigation { get; set; } = default!;

  [Parameter] public Guid? PetWalkerId { get; set; }

  /// <summary>Working week shown in the schedule tab, Monday first.</summary>
  private static readonly DayOfWeek[] WeekDays =
  {
    DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
    DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
  };

  private readonly List<PetWalkerDto> _petWalkers = new();
  private readonly List<ScheduleItemDto> _schedule = new();
  private readonly List<PetWalkerDateExceptionDto> _dateExceptions = new();
  private readonly List<CustomTimeRequestDto> _customRequests = new();

  private AvailabilitySettingsDto _settings = new();
  private AvailabilityTab _activeTab = AvailabilityTab.Schedule;

  private Guid _selectedPetWalkerId = Guid.Empty;
  private bool _isLoading;
  private bool _isSaving;
  private string? _errorMessage;
  private string? _successMessage;

  private bool _showExceptionDialog;
  private bool _showCounterOfferDialog;
  private CustomTimeRequestDto? _selectedRequest;
  private string? _requestStatusFilter;

  private ExceptionFormModel _exceptionForm = new();
  private CounterOfferFormModel _counterOfferForm = new();

  private Guid SelectedPetWalkerId => _selectedPetWalkerId;

  protected override async Task OnInitializedAsync()
  {
    Logger.LogInformation("Availability page initialised. Route PetWalkerId: {PetWalkerId}", PetWalkerId);

    await LoadPetWalkersAsync();

    if (PetWalkerId.HasValue && PetWalkerId.Value != Guid.Empty)
    {
      _selectedPetWalkerId = PetWalkerId.Value;
      await LoadEverythingForSelectionAsync();
    }
  }

  private async Task LoadPetWalkersAsync()
  {
    try
    {
      var response = await PetWalkerService.GetPetWalkersAsync(1, 200);

      _petWalkers.Clear();
      _petWalkers.AddRange((response.RowsData ?? new List<PetWalkerDto>())
          .Where(p => p.Id != Guid.Empty)
          .OrderBy(p => p.Name));

      Logger.LogInformation("Loaded {Count} pet walkers for the availability picker", _petWalkers.Count);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Failed to load pet walkers for the availability page");
      _errorMessage = "Could not load the list of pet walkers.";
    }
  }

  private async Task OnPetWalkerChangedAsync(ChangeEventArgs args)
  {
    var value = args.Value?.ToString();
    _selectedPetWalkerId = Guid.TryParse(value, out var id) ? id : Guid.Empty;

    _successMessage = null;
    _errorMessage = null;

    if (_selectedPetWalkerId != Guid.Empty)
    {
      await LoadEverythingForSelectionAsync();
    }
  }

  private async Task LoadEverythingForSelectionAsync()
  {
    _isLoading = true;
    StateHasChanged();

    try
    {
      await LoadScheduleAsync();
      await LoadDateExceptionsAsync();
      await LoadSettingsAsync();

      if (_activeTab == AvailabilityTab.Requests)
      {
        await LoadCustomRequestsAsync();
      }
    }
    finally
    {
      _isLoading = false;
      StateHasChanged();
    }
  }

  private async Task SetTab(AvailabilityTab tab)
  {
    // Guard against a re-entrant tab switch while a save is in flight.
    if (_isSaving)
    {
      return;
    }

    _activeTab = tab;
    _successMessage = null;
    _errorMessage = null;

    if (tab == AvailabilityTab.Requests && _customRequests.Count == 0)
    {
      await LoadCustomRequestsAsync();
    }
  }

  #region Weekly schedule

  private async Task LoadScheduleAsync()
  {
    try
    {
      var response = await ScheduleService.GetScheduleAsync(_selectedPetWalkerId);

      _schedule.Clear();

      if (response.Success && response.Data != null)
      {
        // Start from a fully populated week so every day has an editable row, then
        // overlay whatever the walker actually has saved.
        foreach (var day in WeekDays)
        {
          var saved = response.Data.Schedules.FirstOrDefault(s => s.DayOfWeek == day);

          _schedule.Add(saved != null
              ? new ScheduleItemDto(day, saved.StartTime, saved.EndTime) { IsActive = true }
              : new ScheduleItemDto(day, new TimeOnly(9, 0), new TimeOnly(17, 0)) { IsActive = false });
        }

        Logger.LogInformation("Loaded {Count} active schedule days for PetWalker {PetWalkerId}",
            _schedule.Count(s => s.IsActive), _selectedPetWalkerId);
      }
      else
      {
        _errorMessage = response.Message ?? "Failed to load the weekly schedule.";
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error loading schedule for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while loading the weekly schedule.";
    }
  }

  private ScheduleItemDto ScheduleFor(DayOfWeek day) =>
      _schedule.First(s => s.DayOfWeek == day);

  private void ToggleDay(DayOfWeek day, ChangeEventArgs args)
  {
    var isActive = args.Value is true;
    ScheduleFor(day).IsActive = isActive;
    _successMessage = null;
  }

  private void SetStartTime(DayOfWeek day, ChangeEventArgs args)
  {
    if (TimeOnly.TryParse(args.Value?.ToString(), out var time))
    {
      ScheduleFor(day).StartTime = time;
      _successMessage = null;
    }
  }

  private void SetEndTime(DayOfWeek day, ChangeEventArgs args)
  {
    if (TimeOnly.TryParse(args.Value?.ToString(), out var time))
    {
      ScheduleFor(day).EndTime = time;
      _successMessage = null;
    }
  }

  /// <summary>
  /// How many slots the configured hours would yield, so the walker sees the effect of
  /// their hours and slot length immediately rather than guessing.
  /// </summary>
  private int SlotCountFor(ScheduleItemDto item)
  {
    var duration = _settings.SlotDurationMinutes > 0
        ? _settings.SlotDurationMinutes
        : DefaultSlotMinutes;

    if (!item.IsActive || item.EndTime <= item.StartTime)
    {
      return 0;
    }

    var totalMinutes = (item.EndTime - item.StartTime).TotalMinutes;

    return (int)(totalMinutes / duration);
  }

  private const int DefaultSlotMinutes = 60;

  private async Task SaveScheduleAsync()
  {
    var invalidDays = _schedule.Where(s => s.IsActive && s.EndTime <= s.StartTime).ToList();

    if (invalidDays.Any())
    {
      _errorMessage = $"End time must be after start time for: {string.Join(", ", invalidDays.Select(d => d.DayOfWeek))}";
      return;
    }

    _isSaving = true;
    _errorMessage = null;

    try
    {
      var response = await ScheduleService.SetScheduleAsync(_selectedPetWalkerId, _schedule);

      if (response.Success)
      {
        _successMessage = $"Weekly schedule saved. Active days: {_schedule.Count(s => s.IsActive)}.";
        await LoadScheduleAsync();
      }
      else
      {
        _errorMessage = BuildError(response.Errors, response.Message, "Failed to save the weekly schedule.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error saving schedule for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while saving the weekly schedule.";
    }
    finally
    {
      _isSaving = false;
      StateHasChanged();
    }
  }

  #endregion

  #region Date exceptions

  private async Task LoadDateExceptionsAsync()
  {
    try
    {
      var response = await ScheduleService.GetDateExceptionsAsync(_selectedPetWalkerId);

      _dateExceptions.Clear();

      if (response.Success && response.Data != null)
      {
        _dateExceptions.AddRange(response.Data);
      }
      else if (!response.Success)
      {
        _errorMessage = BuildError(response.Errors, response.Message, "Failed to load date exceptions.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error loading date exceptions for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while loading date exceptions.";
    }
  }

  private void OpenExceptionDialog()
  {
    _exceptionForm = new ExceptionFormModel
    {
      Date = DateTime.Today.AddDays(1),
      IsUnavailable = true,
      StartTime = new TimeOnly(9, 0),
      EndTime = new TimeOnly(17, 0)
    };

    _showExceptionDialog = true;
  }

  private void CloseExceptionDialog() => _showExceptionDialog = false;

  private async Task SaveExceptionAsync()
  {
    if (_exceptionForm.Date == default)
    {
      _errorMessage = "Please choose a date for the exception.";
      return;
    }

    if (!_exceptionForm.IsUnavailable && _exceptionForm.EndTime <= _exceptionForm.StartTime)
    {
      _errorMessage = "End time must be after start time.";
      return;
    }

    _isSaving = true;
    _errorMessage = null;

    try
    {
      var response = await ScheduleService.SetDateExceptionAsync(
          _selectedPetWalkerId,
          _exceptionForm.Date,
          _exceptionForm.IsUnavailable,
          _exceptionForm.IsUnavailable ? null : _exceptionForm.StartTime,
          _exceptionForm.IsUnavailable ? null : _exceptionForm.EndTime,
          string.IsNullOrWhiteSpace(_exceptionForm.Reason) ? null : _exceptionForm.Reason);

      if (response.Success)
      {
        _successMessage = "Date exception saved.";
        _showExceptionDialog = false;
        await LoadDateExceptionsAsync();
      }
      else
      {
        _errorMessage = BuildError(response.Errors, response.Message, "Failed to save the date exception.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error saving date exception for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while saving the date exception.";
    }
    finally
    {
      _isSaving = false;
      StateHasChanged();
    }
  }

  private async Task DeleteExceptionAsync(PetWalkerDateExceptionDto exception)
  {
    _isSaving = true;
    _errorMessage = null;

    try
    {
      var response = await ScheduleService.DeleteDateExceptionAsync(
          _selectedPetWalkerId, exception.Date.ToDateTime(TimeOnly.MinValue));

      if (response.Success)
      {
        _successMessage = "Date exception removed.";
        await LoadDateExceptionsAsync();
      }
      else
      {
        _errorMessage = BuildError(response.Errors, response.Message, "Failed to remove the date exception.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error deleting date exception for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while removing the date exception.";
    }
    finally
    {
      _isSaving = false;
      StateHasChanged();
    }
  }

  #endregion

  #region Settings

  private async Task LoadSettingsAsync()
  {
    try
    {
      var response = await ScheduleService.GetAvailabilitySettingsAsync(_selectedPetWalkerId);

      if (response.Success && response.Data != null)
      {
        _settings = response.Data;

        // A legacy row could carry 0; show the effective default rather than a misleading 0.
        if (_settings.SlotDurationMinutes <= 0)
        {
          _settings.SlotDurationMinutes = DefaultSlotMinutes;
        }
      }
      else
      {
        _errorMessage = BuildError(response.Errors, response.Message, "Failed to load availability settings.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error loading availability settings for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while loading availability settings.";
    }
  }

  private async Task SaveSettingsAsync()
  {
    _isSaving = true;
    _errorMessage = null;

    try
    {
      var response = await ScheduleService.UpdateAvailabilitySettingsAsync(
          _selectedPetWalkerId,
          _settings.SlotDurationMinutes,
          _settings.InterBookingGapMinutes,
          _settings.DailyPetWalkLimit);

      if (response.Success && response.Data != null)
      {
        _settings = response.Data;
        _successMessage = "Availability settings updated.";
      }
      else
      {
        _errorMessage = BuildError(response.Errors, response.Message, "Failed to update availability settings.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error updating availability settings for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while updating availability settings.";
    }
    finally
    {
      _isSaving = false;
      StateHasChanged();
    }
  }

  #endregion

  #region Custom requests

  private async Task LoadCustomRequestsAsync()
  {
    try
    {
      var response = await TimeslotService.GetPetWalkerCustomTimeRequestsAsync(
          _selectedPetWalkerId, _requestStatusFilter);

      _customRequests.Clear();

      if (response.Success && response.Data != null)
      {
        _customRequests.AddRange(response.Data);
      }
      else if (!response.Success)
      {
        _errorMessage = BuildError(response.Errors, response.Message, "Failed to load custom time requests.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error loading custom time requests for PetWalker {PetWalkerId}", _selectedPetWalkerId);
      _errorMessage = "An unexpected error occurred while loading custom time requests.";
    }
  }

  private async Task OnStatusFilterChanged(ChangeEventArgs args)
  {
    _requestStatusFilter = string.IsNullOrWhiteSpace(args.Value?.ToString()) ? null : args.Value!.ToString();
    await LoadCustomRequestsAsync();
  }

  private async Task AcceptRequest(Guid requestId)
  {
    await RespondToRequestAsync(requestId, "Accept");
  }

  private async Task DeclineRequest(Guid requestId)
  {
    await RespondToRequestAsync(requestId, "Decline");
  }

  private async Task RespondToRequestAsync(Guid requestId, string response)
  {
    _isSaving = true;
    _errorMessage = null;

    try
    {
      var result = await TimeslotService.RespondToCustomTimeRequestAsync(
          new RespondToCustomTimeRequestRequest
          {
            RequestId = requestId,
            Response = response
          });

      if (result.Success)
      {
        _successMessage = $"Request {response.ToLowerInvariant()}d.";
        await LoadCustomRequestsAsync();
      }
      else
      {
        _errorMessage = BuildError(result.Errors, result.Message, "Failed to respond to the request.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error responding to custom time request {RequestId}", requestId);
      _errorMessage = "An unexpected error occurred while responding to the request.";
    }
    finally
    {
      _isSaving = false;
      StateHasChanged();
    }
  }

  private void ShowCounterOfferDialog(CustomTimeRequestDto request)
  {
    _selectedRequest = request;
    _counterOfferForm = new CounterOfferFormModel
    {
      Date = request.RequestedDate.ToDateTime(TimeOnly.MinValue),
      StartTime = request.PreferredStartTime,
      DurationMinutes = request.PreferredDurationMinutes
    };

    _showCounterOfferDialog = true;
  }

  private void CloseCounterOfferDialog()
  {
    _showCounterOfferDialog = false;
    _selectedRequest = null;
  }

  private async Task SubmitCounterOffer()
  {
    if (_selectedRequest == null)
    {
      return;
    }

    _isSaving = true;
    _errorMessage = null;

    try
    {
      var result = await TimeslotService.RespondToCustomTimeRequestAsync(
          new RespondToCustomTimeRequestRequest
          {
            RequestId = _selectedRequest.RequestId,
            Response = "CounterOffer",
            CounterOfferedDate = DateOnly.FromDateTime(_counterOfferForm.Date),
            CounterOfferedTime = _counterOfferForm.StartTime
          });

      if (result.Success)
      {
        _successMessage = "Counter offer sent.";
        CloseCounterOfferDialog();
        await LoadCustomRequestsAsync();
      }
      else
      {
        _errorMessage = BuildError(result.Errors, result.Message, "Failed to send the counter offer.");
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error sending counter offer");
      _errorMessage = "An unexpected error occurred while sending the counter offer.";
    }
    finally
    {
      _isSaving = false;
      StateHasChanged();
    }
  }

  private static string RequestStatusClass(string? status) => status?.ToLowerInvariant() switch
  {
    "pending" => "warning",
    "accepted" => "success",
    "declined" => "danger",
    "counteroffered" => "info",
    _ => "neutral"
  };

  #endregion

  private void ClearSuccess() => _successMessage = null;

  private static string BuildError(List<string>? errors, string? fallback, string defaultMessage)
  {
    if (errors != null && errors.Count > 0)
    {
      return string.Join(" ", errors);
    }

    return string.IsNullOrWhiteSpace(fallback) ? defaultMessage : fallback;
  }

  private enum AvailabilityTab
  {
    Schedule,
    Exceptions,
    Settings,
    Requests
  }

  private class ExceptionFormModel
  {
    public DateTime Date { get; set; } = DateTime.Today.AddDays(1);
    public bool IsUnavailable { get; set; } = true;
    public TimeOnly StartTime { get; set; } = new(9, 0);
    public TimeOnly EndTime { get; set; } = new(17, 0);
    public string? Reason { get; set; }
  }

  private class CounterOfferFormModel
  {
    public DateTime Date { get; set; } = DateTime.Today.AddDays(1);
    public TimeOnly StartTime { get; set; } = new(9, 0);
    public int DurationMinutes { get; set; } = 30;
  }
}
