using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Pages.Bookings;

public partial class BookingManagement
{
  [Inject] public ILogger<BookingManagement> Logger { get; set; } = default!;
  [Inject] public IBookingService BookingService { get; set; } = default!;
  [Inject] public IClientService ClientService { get; set; } = default!;
  [Inject] public NavigationManager Navigation { get; set; } = default!;
  [Inject] public IPopupService PopupService { get; set; } = default!;

  [Parameter] public Guid? ClientId { get; set; }

  [SupplyParameterFromQuery]
  private Guid? petWalkerId { get; set; }

  private List<PetWalkerSummaryDto>? availablePetWalkers;
  private PetWalkerSummaryDto? selectedPetWalker;
  private ClientDto? clientInfo;
  private bool showBookingForm = false;
  private bool isLoadingPetWalkers = false;
  private string? errorMessage;

  // Success modal state
  private bool showSuccessModal = false;
  private BookingResponseDto? completedBooking;

  // Pagination and filtering state
  private PaginatedPetWalkersResponse? _paginatedResponse;
  private GetAvailablePetWalkersRequest _currentRequest = new()
  {
    Page = 1,
    PageSize = 15,
    SortBy = "name",
    SortDirection = "asc"
  };
  private List<string> _availableServiceAreas = new();
  private string _selectedServiceArea = string.Empty;
  private string _searchTerm = string.Empty;
  private string _selectedSortBy = "name";
  private string _selectedSortDirection = "asc";



  protected override async Task OnInitializedAsync()
  {
    Logger.LogInformation("INIT: BookingManagement page initialized with ClientId: {ClientId}", ClientId);
    Logger.LogInformation("INIT: Page route is /bookings or /bookings/new - showBookingForm will be: {ShowForm}", 
      ClientId.HasValue);

    if (ClientId.HasValue)
    {
      await LoadClientInfoAsync(ClientId.Value);
      showBookingForm = true;
      Logger.LogInformation("INIT: ClientId provided - will show booking form");
    }
    else
    {
      await LoadPetWalkersAsync();
      Logger.LogInformation("INIT: No ClientId - will show pet walker selection (showBookingForm=false)");
    }
  }

  protected override async Task OnParametersSetAsync()
  {
    Logger.LogInformation("PARAM SET: ClientId: {ClientId}, petWalkerId: {PetWalkerId}, current selectedPetWalker: {Selected}",
      ClientId, petWalkerId, selectedPetWalker?.FullName ?? "null");

    if (ClientId.HasValue && (clientInfo == null || clientInfo.Id != ClientId.Value))
    {
      await LoadClientInfoAsync(ClientId.Value);
      showBookingForm = true;
    }

    // Handle petWalkerId from URL query parameter
    if (petWalkerId.HasValue)
    {
      Logger.LogInformation("PARAM SET: petWalkerId found in URL - {PetWalkerId}", petWalkerId.Value);
      
      // If pet walkers are already loaded, try to find and select the pet walker
      if (availablePetWalkers != null && availablePetWalkers.Any())
      {
        var petWalker = availablePetWalkers.FirstOrDefault(p => p.Id == petWalkerId.Value);
        if (petWalker != null)
        {
          Logger.LogInformation("PARAM SET: Found pet walker in list - {PetWalkerName}", petWalker.FullName);
          selectedPetWalker = petWalker;
          showBookingForm = true;
        }
        else
        {
          Logger.LogWarning("PARAM SET: petWalkerId {PetWalkerId} not found in available pet walkers", petWalkerId.Value);
        }
      }
      else
      {
        // Pet walkers not loaded yet - will need to load and then select
        Logger.LogInformation("PARAM SET: Pet walkers not loaded yet, will select after loading");
        showBookingForm = true;
      }
    }
  }

  private async Task LoadPetWalkersAsync()
  {
    try
    {
      isLoadingPetWalkers = true;
      errorMessage = null;
      Logger.LogInformation("LOAD PETWALKERS: Starting to load pet walkers - Page: {Page}, PageSize: {PageSize}", 
        _currentRequest.Page, _currentRequest.PageSize);
      StateHasChanged();

      Logger.LogInformation("LOAD PETWALKERS: Current filters - ServiceArea: {ServiceArea}, SearchTerm: {SearchTerm}, SortBy: {SortBy}",
        _selectedServiceArea, _searchTerm, _selectedSortBy);

      // Update request with current filter/sort values
      _currentRequest.ServiceArea = _selectedServiceArea;
      _currentRequest.SearchTerm = _searchTerm;
      _currentRequest.SortBy = _selectedSortBy;
      _currentRequest.SortDirection = _selectedSortDirection;

      var response = await BookingService.GetAvailablePetWalkersAsync(_currentRequest);
      if (response.Success && response.Data != null)
      {
        _paginatedResponse = response.Data;
        availablePetWalkers = response.Data.PetWalkers;
        _availableServiceAreas = response.Data.AvailableServiceAreas;

        Logger.LogInformation("LOAD PETWALKERS SUCCESS: Loaded {Count} pet walkers (Page {Page} of {TotalPages}). Total available: {TotalCount}",
          availablePetWalkers.Count, response.Data.PageNumber, response.Data.TotalPages, response.Data.TotalCount);

        // If petWalkerId was in URL, try to select it after loading
        if (petWalkerId.HasValue && selectedPetWalker == null)
        {
          var petWalker = availablePetWalkers.FirstOrDefault(p => p.Id == petWalkerId.Value);
          if (petWalker != null)
          {
            Logger.LogInformation("LOAD PETWALKERS: Auto-selecting pet walker from URL - {PetWalkerId} - {PetWalkerName}",
              petWalker.Id, petWalker.FullName);
            selectedPetWalker = petWalker;
            showBookingForm = true;
          }
        }
      }
      else
      {
        availablePetWalkers = new List<PetWalkerSummaryDto>();
        _paginatedResponse = null;
        errorMessage = response.Message ?? "Failed to load pet walkers";
        Logger.LogWarning("LOAD PETWALKERS FAILED: {Error}", errorMessage);
      }
    }
    catch (Exception ex)
    {
      availablePetWalkers = new List<PetWalkerSummaryDto>();
      _paginatedResponse = null;
      errorMessage = "An error occurred while loading pet walkers";
      Logger.LogError(ex, "LOAD PETWALKERS ERROR: Exception occurred");
    }
    finally
    {
      isLoadingPetWalkers = false;
      Logger.LogInformation("LOAD PETWALKERS COMPLETE: availablePetWalkers count: {Count}, errorMessage: {Error}", 
        availablePetWalkers?.Count ?? 0, errorMessage ?? "none");
      StateHasChanged();
    }
  }

  private async Task LoadClientInfoAsync(Guid clientId)
  {
    try
    {
      Logger.LogInformation("Loading client info: {ClientId}", clientId);

      var response = await ClientService.GetClientAsync(clientId);
      if (response.Success && response.Data != null)
      {
        clientInfo = ClientService.MapClientDataToDto(response.Data);
        Logger.LogInformation("Successfully loaded client: {ClientName}", response.Data.Name);
      }
      else
      {
        errorMessage = response.Message ?? "Failed to load client information";
        Logger.LogWarning("Failed to load client {ClientId}: {Error}", clientId, errorMessage);
      }
    }
    catch (Exception ex)
    {
      errorMessage = "An error occurred while loading client information";
      Logger.LogError(ex, "Error loading client {ClientId}", clientId);
    }
  }

  private void StartNewBooking()
  {
    try
    {
      Logger.LogInformation("START BOOKING: New Booking button clicked. Current state - showBookingForm: {ShowForm}, selectedPetWalker: {Selected}", 
        showBookingForm, selectedPetWalker?.FullName ?? "null");
      
      showBookingForm = false;  // This will show pet walker selection (not form)
      selectedPetWalker = null;
      ClientId = null;
      Logger.LogInformation("START BOOKING: Navigating to /bookings/new. showBookingForm will be: {ShowForm}", showBookingForm);
      Navigation.NavigateTo("/bookings/new");
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error starting new booking");
    }
  }

  private void SelectPetWalker(PetWalkerSummaryDto petWalker)
  {
    try
    {
      Logger.LogInformation("PETWALKER CLICK: Attempting to select pet walker: {PetWalkerId} - {PetWalkerName}", petWalker.Id, petWalker.FullName);
      Logger.LogInformation("PETWALKER CLICK: Current selectedPetWalker before selection: {CurrentSelected}", selectedPetWalker?.FullName ?? "null");

      // Toggle selection - if same pet walker is clicked, deselect
      if (selectedPetWalker?.Id == petWalker.Id)
      {
        Logger.LogInformation("PETWALKER CLICK: Deselecting pet walker (same was already selected)");
        selectedPetWalker = null;
      }
      else
      {
        Logger.LogInformation("PETWALKER CLICK: Selecting new pet walker");
        selectedPetWalker = petWalker;
      }

      Logger.LogInformation("PETWALKER CLICK: selectedPetWalker after selection: {NewSelected}", selectedPetWalker?.FullName ?? "null");
      StateHasChanged();
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error selecting pet walker: {PetWalkerId}", petWalker.Id);
    }
  }

  private void ViewPetWalkerDetails(PetWalkerSummaryDto petWalker)
  {
    try
    {
      Logger.LogInformation("Viewing pet walker details: {PetWalkerId} - {PetWalkerName}", petWalker.Id, petWalker.FullName);

      // Use the PopupService to show the PetWalkerViewPopup
      PopupService.ShowViewPetWalkerPopup(petWalker.Email);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error viewing pet walker details: {PetWalkerId}", petWalker.Id);
    }
  }

  private void ProceedToBooking()
  {
    try
    {
      Logger.LogInformation("PROCEED TO BOOKING: Book Now button clicked. selectedPetWalker: {PetWalkerId} - {PetWalkerName}", 
        selectedPetWalker?.Id, selectedPetWalker?.FullName ?? "null");

      if (selectedPetWalker == null)
      {
        Logger.LogWarning("PROCEED TO BOOKING: Attempted to proceed to booking without selected pet walker");
        return;
      }

      Logger.LogInformation("PROCEED TO BOOKING: Setting showBookingForm = true and navigating to booking form");
      showBookingForm = true;
      StateHasChanged();

      // Update URL to reflect selected pet walker
      Navigation.NavigateTo($"/bookings/new?petWalkerId={selectedPetWalker.Id}");
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error proceeding to booking");
    }
  }

  private void ChangePetWalker()
  {
    Logger.LogInformation("User requested to change pet walker");
    showBookingForm = false;
    selectedPetWalker = null;
    Navigation.NavigateTo("/bookings/new");
  }

  private Task OnBookingCompleted(BookingResponseDto bookingResponse)
  {
    try
    {
      Logger.LogInformation("Booking completed successfully: {BookingId}", bookingResponse.BookingId);

      completedBooking = bookingResponse;
      showSuccessModal = true;
      StateHasChanged();

      return Task.CompletedTask;
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error handling booking completion");
      return Task.CompletedTask;
    }
  }

  private Task OnBookingCancelled()
  {
    try
    {
      Logger.LogInformation("Booking cancelled by user");

      // Navigate back to client selection or main bookings page
      if (ClientId.HasValue)
      {
        Navigation.NavigateTo("/bookings");
      }
      else
      {
        showBookingForm = false;
        StateHasChanged();
      }

      return Task.CompletedTask;
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error handling booking cancellation");
      return Task.CompletedTask;
    }
  }

  private void CloseSuccessModal()
  {
    showSuccessModal = false;
    completedBooking = null;
    StateHasChanged();
  }

  private void CreateAnotherBooking()
  {
    Logger.LogInformation("User requested to create another booking");

    CloseSuccessModal();

    // Keep the same pet walker if one was selected
    if (selectedPetWalker != null)
    {
      showBookingForm = true;
    }
    else
    {
      showBookingForm = false;
    }

    StateHasChanged();
  }

  private void NavigateToPetWalkers()
  {
    Logger.LogInformation("Navigating to pet walkers management");
    Navigation.NavigateTo("/petwalkers");
  }

  private static string TruncateText(string text, int maxLength)
  {
    if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
      return text;

    return text[..maxLength].TrimEnd() + "...";
  }

  #region Pagination, Filtering, and Sorting Methods

  private async Task OnPageChanged(int newPage)
  {
    if (newPage != _currentRequest.Page && newPage > 0 &&
        (_paginatedResponse == null || newPage <= _paginatedResponse.TotalPages))
    {
      Logger.LogInformation("Changing to page {NewPage} from {CurrentPage}", newPage, _currentRequest.Page);
      _currentRequest.Page = newPage;
      await LoadPetWalkersAsync();
    }
  }

  private async Task OnPageSizeChanged(int newPageSize)
  {
    if (newPageSize != _currentRequest.PageSize && newPageSize > 0)
    {
      _currentRequest.PageSize = newPageSize;
      _currentRequest.Page = 1; // Reset to first page
      await LoadPetWalkersAsync();
    }
  }

  private async Task OnServiceAreaChanged(string serviceArea)
  {
    if (_selectedServiceArea != serviceArea)
    {
      _selectedServiceArea = serviceArea;
      _currentRequest.Page = 1; // Reset to first page
      await LoadPetWalkersAsync();
    }
  }

  private async Task OnSearchTermChanged(string searchTerm)
  {
    if (_searchTerm != searchTerm)
    {
      _searchTerm = searchTerm;
      _currentRequest.Page = 1; // Reset to first page
      await LoadPetWalkersAsync();
    }
  }

  private async Task OnSortChanged(string sortBy, string sortDirection)
  {
    if (_selectedSortBy != sortBy || _selectedSortDirection != sortDirection)
    {
      _selectedSortBy = sortBy;
      _selectedSortDirection = sortDirection;
      await LoadPetWalkersAsync();
    }
  }

  private async Task ClearFilters()
  {
    _selectedServiceArea = string.Empty;
    _searchTerm = string.Empty;
    _selectedSortBy = "name";
    _selectedSortDirection = "asc";
    _currentRequest.Page = 1;
    await LoadPetWalkersAsync();
  }

  #endregion
}
