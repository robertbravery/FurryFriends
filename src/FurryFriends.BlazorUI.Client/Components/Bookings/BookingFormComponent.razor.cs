using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Components.Bookings;

public partial class BookingFormComponent
{
  [Inject] public IBookingService BookingService { get; set; } = default!;
  [Inject] public IClientService ClientService { get; set; } = default!;
  [Inject] public IClientContextService ClientContextService { get; set; } = default!;
  [Inject] public ILogger<BookingFormComponent> Logger { get; set; } = default!;

  [Parameter] public Guid? ClientId { get; set; }
  [Parameter] public string? ServiceArea { get; set; }
  [Parameter] public PetWalkerSummaryDto? SelectedPetWalker { get; set; }
  [Parameter] public EventCallback<BookingResponseDto> OnBookingCompleted { get; set; }
  [Parameter] public EventCallback OnBookingCancelled { get; set; }

  private BookingRequestDto bookingRequest = new();
  private PetWalkerSummaryDto? selectedPetWalker;
  private PetDto[]? availablePets;
  private DateTime? selectedDate;
  private DateTime? selectedStartTime;
  private DateTime? selectedEndTime;

  private int currentStep = 1;
  private bool isSubmitting = false;
  private string? errorMessage;
  private string? successMessage;

  /// <summary>
  /// The pet owner this booking is for. Always the current client - supplied as a parameter,
  /// or resolved from the client context. There is deliberately no client picker: the person
  /// making the booking is the client.
  /// </summary>
  private Guid _petOwnerId = Guid.Empty;

  /// <summary>
  /// Set when the pet lookup itself failed, so the user is told to retry rather than
  /// being shown "this client has no pets".
  /// </summary>
  private string? _petLoadError;

  private bool HasPetError => bookingRequest.PetId == Guid.Empty && currentStep >= 3;
  private bool HasPetOwner => _petOwnerId != Guid.Empty;

  protected override async Task OnInitializedAsync()
  {
    await ResolvePetOwnerAsync();
  }

  /// <summary>
  /// Determines whose booking this is: an explicitly supplied ClientId, otherwise the
  /// current client from context. If neither yields a client the booking cannot proceed,
  /// which is reported on the details step rather than silently showing an empty pet list.
  /// </summary>
  private async Task ResolvePetOwnerAsync()
  {
    var resolved = ClientId ?? Guid.Empty;

    if (resolved == Guid.Empty)
    {
      try
      {
        resolved = await ClientContextService.GetCurrentClientIdAsync();
      }
      catch (Exception ex)
      {
        // The context provider is a seam and may not be available in every host.
        Logger.LogWarning(ex, "Could not read the current client from context");
      }
    }

    if (resolved == Guid.Empty)
    {
      Logger.LogWarning("No current client is available; the booking cannot be attributed to an owner");
      return;
    }

    await SetPetOwnerAsync(resolved);
  }

  private async Task SetPetOwnerAsync(Guid clientId)
  {
    _petOwnerId = clientId;
    bookingRequest.PetOwnerId = clientId;
    bookingRequest.PetId = Guid.Empty;
    availablePets = null;

    await LoadClientPetsAsync();
    StateHasChanged();
  }

  private async Task LoadClientPetsAsync()
  {
    if (_petOwnerId == Guid.Empty) return;

    try
    {
      Logger.LogInformation("Loading pets for client: {ClientId}", _petOwnerId);
      _petLoadError = null;

      var response = await ClientService.GetClientAsync(_petOwnerId);
      if (response.Success && response.Data != null)
      {
        availablePets = response.Data.Pets ?? Array.Empty<PetDto>();
        Logger.LogInformation("Successfully loaded {Count} pets", availablePets.Length);
      }
      else
      {
        availablePets = Array.Empty<PetDto>();
        _petLoadError = "Could not load this client's pets. Please try again.";
        Logger.LogWarning("Failed to load client pets: {Error}", response.Message);
      }
    }
    catch (Exception ex)
    {
      availablePets = Array.Empty<PetDto>();
      // Distinguish "the lookup failed" from "there are genuinely no pets".
      _petLoadError = "Could not load this client's pets. Please try again.";
      Logger.LogError(ex, "Error loading client pets");
    }
  }

  /// <summary>Re-runs the pet lookup after a transient failure.</summary>
  private async Task RetryLoadPetsAsync()
  {
    await LoadClientPetsAsync();
    StateHasChanged();
  }

  protected override async Task OnParametersSetAsync()
  {
    // Handle ClientId change
    if (ClientId.HasValue && ClientId.Value != _petOwnerId)
    {
      await SetPetOwnerAsync(ClientId.Value);
    }

    // Handle pre-selected pet walker - auto-advance to step 2
    if (SelectedPetWalker != null && selectedPetWalker == null)
    {
      Logger.LogInformation("Pre-selected pet walker received: {PetWalkerId} - {PetWalkerName}", 
        SelectedPetWalker.Id, SelectedPetWalker.FullName);
      
      selectedPetWalker = SelectedPetWalker;
      bookingRequest.PetWalkerId = SelectedPetWalker.Id;
      
      // Auto-advance to step 2 if we're still on step 1
      if (currentStep == 1)
      {
        currentStep = 2;
        Logger.LogInformation("Auto-advancing to step 2 due to pre-selected pet walker");
      }
    }
  }

  private Task OnPetWalkerSelected(PetWalkerSummaryDto petWalker)
  {
    selectedPetWalker = petWalker;
    bookingRequest.PetWalkerId = petWalker.Id;
    Logger.LogInformation("PetWalker selected: {PetWalkerId} - {Name}", petWalker.Id, petWalker.FullName);
    StateHasChanged();
    return Task.CompletedTask;
  }

  private Task OnPetWalkerIdChanged(Guid? petWalkerId)
  {
    if (petWalkerId.HasValue)
    {
      bookingRequest.PetWalkerId = petWalkerId.Value;
    }
    return Task.CompletedTask;
  }

  private Task OnDateChanged(DateTime? date)
  {
    selectedDate = date;
    // Clear time selections when date changes
    selectedStartTime = null;
    selectedEndTime = null;
    UpdateBookingRequestDates();
    return Task.CompletedTask;
  }

  private Task OnStartTimeChanged(DateTime? startTime)
  {
    selectedStartTime = startTime;
    UpdateBookingRequestDates();
    return Task.CompletedTask;
  }

  private Task OnEndTimeChanged(DateTime? endTime)
  {
    selectedEndTime = endTime;
    UpdateBookingRequestDates();
    return Task.CompletedTask;
  }

  private Task OnTimeSelectionChanged((DateTime startTime, DateTime endTime) timeSelection)
  {
    selectedStartTime = timeSelection.startTime;
    selectedEndTime = timeSelection.endTime;
    UpdateBookingRequestDates();
    return Task.CompletedTask;
  }

  private void UpdateBookingRequestDates()
  {
    if (selectedStartTime.HasValue && selectedEndTime.HasValue)
    {
      bookingRequest.StartDate = selectedStartTime.Value;
      bookingRequest.EndDate = selectedEndTime.Value;
      bookingRequest.Price = CalculatePrice();
    }
  }

  private void NextStep()
  {
    if (ValidateCurrentStep())
    {
      currentStep++;
      ClearMessages();
      StateHasChanged();
    }
  }

  private void PreviousStep()
  {
    if (currentStep > 1)
    {
      currentStep--;
      ClearMessages();
      StateHasChanged();
    }
  }

  private bool ValidateCurrentStep()
  {
    switch (currentStep)
    {
      case 1:
        if (bookingRequest.PetWalkerId == Guid.Empty)
        {
          errorMessage = "Please select a pet walker";
          return false;
        }
        break;
      case 2:
        if (!HasValidTimeSelection())
        {
          errorMessage = "Please select a date and time for the booking";
          return false;
        }
        break;
      case 3:
        if (!IsBookingDetailsValid())
        {
          errorMessage = "Please complete all required booking details";
          return false;
        }
        break;
    }
    return true;
  }

  private bool HasValidTimeSelection()
  {
    return selectedDate.HasValue && selectedStartTime.HasValue && selectedEndTime.HasValue;
  }

  private bool IsBookingDetailsValid()
  {
    return bookingRequest.PetId != Guid.Empty && HasValidTimeSelection();
  }

  private decimal CalculatePrice()
  {
    if (selectedPetWalker == null || !selectedStartTime.HasValue || !selectedEndTime.HasValue)
      return 0;

    var duration = selectedEndTime.Value - selectedStartTime.Value;
    var hours = (decimal)duration.TotalHours;
    return Math.Round(hours * selectedPetWalker.HourlyRate, 2);
  }

  private string GetTimeRangeDisplay()
  {
    if (!selectedStartTime.HasValue || !selectedEndTime.HasValue)
      return "Not selected";

    return $"{selectedStartTime.Value:HH:mm} - {selectedEndTime.Value:HH:mm}";
  }

  private string GetDurationDisplay()
  {
    if (!selectedStartTime.HasValue || !selectedEndTime.HasValue)
      return "Not selected";

    var duration = selectedEndTime.Value - selectedStartTime.Value;
    return $"{(int)duration.TotalMinutes} minutes";
  }

  private string GetTimelineProgress() =>
    currentStep switch
    {
      1 => "0%",
      2 => "33.333%",
      3 => "66.666%",
      _ => "100%"
    };

  private PetDto? GetSelectedPet()
  {
    return availablePets?.FirstOrDefault(p => p.Id == bookingRequest.PetId);
  }

  private async Task SubmitBooking()
  {
    try
    {
      isSubmitting = true;
      ClearMessages();
      StateHasChanged();

      Logger.LogInformation("Submitting booking request for PetWalker: {PetWalkerId}, Client: {ClientId}",
          bookingRequest.PetWalkerId, bookingRequest.PetOwnerId);

      var response = await BookingService.CreateBookingAsync(bookingRequest);

      if (response.Success && response.Data != null)
      {
        successMessage = "Booking created successfully!";
        Logger.LogInformation("Successfully created booking: {BookingId}", response.Data.BookingId);

        // What happens next is the parent's decision: dismiss to the bookings list, or start a
        // new booking (which rebuilds this component). Resetting here on a timer used to clear
        // the form while the confirmation was still open, leaving a half-empty step behind it.
        await OnBookingCompleted.InvokeAsync(response.Data);
      }
      else
      {
        errorMessage = response.Message ?? "Failed to create booking. Please try again.";
        Logger.LogWarning("Failed to create booking: {Error}", errorMessage);
      }
    }
    catch (Exception ex)
    {
      errorMessage = "An error occurred while creating the booking. Please try again.";
      Logger.LogError(ex, "Error submitting booking");
    }
    finally
    {
      isSubmitting = false;
      StateHasChanged();
    }
  }

  private void ClearMessages()
  {
    errorMessage = null;
    successMessage = null;
  }

  private void ClearError()
  {
    errorMessage = null;
    StateHasChanged();
  }

  private void ClearSuccess()
  {
    successMessage = null;
    StateHasChanged();
  }

  public void SetClientId(Guid clientId)
  {
    ClientId = clientId;
    _petOwnerId = clientId;
    bookingRequest.PetOwnerId = clientId;
    _ = LoadClientPetsAsync();
  }

  public void SetServiceArea(string serviceArea)
  {
    ServiceArea = serviceArea;
    StateHasChanged();
  }

  public void Cancel()
  {
    _ = OnBookingCancelled.InvokeAsync();
  }
}
