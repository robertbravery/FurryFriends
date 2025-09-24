using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Components.Bookings;

public partial class BookingFormComponent
{
  [Inject] public IBookingService BookingService { get; set; } = default!;
  [Inject] public IClientService ClientService { get; set; } = default!;
  [Inject] public ILogger<BookingFormComponent> Logger { get; set; } = default!;

  [Parameter] public Guid? ClientId { get; set; }
  [Parameter] public string? ServiceArea { get; set; }
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

  // Validation properties
  private bool HasPetError => bookingRequest.PetId == Guid.Empty && currentStep >= 3;

  protected override async Task OnInitializedAsync()
  {
    if (ClientId.HasValue)
    {
      bookingRequest.PetOwnerId = ClientId.Value;
      await LoadClientPetsAsync();
    }
  }

  protected override async Task OnParametersSetAsync()
  {
    if (ClientId.HasValue && ClientId != bookingRequest.PetOwnerId)
    {
      bookingRequest.PetOwnerId = ClientId.Value;
      await LoadClientPetsAsync();
    }
  }

  private async Task LoadClientPetsAsync()
  {
    if (!ClientId.HasValue) return;

    try
    {
      Logger.LogInformation("Loading pets for client: {ClientId}", ClientId);

      var response = await ClientService.GetClientAsync(ClientId.Value);
      if (response.Success && response.Data != null)
      {
        availablePets = response.Data.Pets;
        Logger.LogInformation("Successfully loaded {Count} pets", availablePets.Length);
      }
      else
      {
        availablePets = Array.Empty<PetDto>();
        Logger.LogWarning("Failed to load client pets: {Error}", response.Message);
      }
    }
    catch (Exception ex)
    {
      availablePets = Array.Empty<PetDto>();
      Logger.LogError(ex, "Error loading client pets");
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

        await OnBookingCompleted.InvokeAsync(response.Data);

        // Reset form after successful submission
        await Task.Delay(2000); // Show success message for 2 seconds
        ResetForm();
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

  private void ResetForm()
  {
    bookingRequest = new BookingRequestDto { PetOwnerId = ClientId ?? Guid.Empty };
    selectedPetWalker = null;
    selectedDate = null;
    selectedStartTime = null;
    selectedEndTime = null;
    currentStep = 1;
    ClearMessages();
    StateHasChanged();
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
