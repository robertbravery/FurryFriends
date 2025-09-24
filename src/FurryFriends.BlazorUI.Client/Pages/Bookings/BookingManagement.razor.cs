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
    Logger.LogInformation("BookingManagement page initialized with ClientId: {ClientId}", ClientId);

    if (ClientId.HasValue)
    {
      await LoadClientInfoAsync(ClientId.Value);
      showBookingForm = true;
    }
    else
    {
      await LoadPetWalkersAsync();
    }
  }

  protected override async Task OnParametersSetAsync()
  {
    if (ClientId.HasValue && (clientInfo == null || clientInfo.Id != ClientId.Value))
    {
      await LoadClientInfoAsync(ClientId.Value);
      showBookingForm = true;
    }
  }

  private async Task LoadPetWalkersAsync()
  {
    try
    {
      isLoadingPetWalkers = true;
      errorMessage = null;
      StateHasChanged();

      Logger.LogInformation("Loading available pet walkers - Page: {Page}, PageSize: {PageSize}",
        _currentRequest.Page, _currentRequest.PageSize);

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

        Logger.LogInformation("Successfully loaded {Count} pet walkers (Page {Page} of {TotalPages}). Total available: {TotalCount}",
          availablePetWalkers.Count, response.Data.PageNumber, response.Data.TotalPages, response.Data.TotalCount);
      }
      else
      {
        availablePetWalkers = new List<PetWalkerSummaryDto>();
        _paginatedResponse = null;
        errorMessage = response.Message ?? "Failed to load pet walkers";
        Logger.LogWarning("Failed to load pet walkers: {Error}", errorMessage);
      }
    }
    catch (Exception ex)
    {
      availablePetWalkers = new List<PetWalkerSummaryDto>();
      _paginatedResponse = null;
      errorMessage = "An error occurred while loading pet walkers";
      Logger.LogError(ex, "Error loading pet walkers");
    }
    finally
    {
      isLoadingPetWalkers = false;
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
    Logger.LogInformation("Starting new booking process");
    showBookingForm = false;
    selectedPetWalker = null;
    ClientId = null;
    Navigation.NavigateTo("/bookings/new");
  }

  private void SelectPetWalker(PetWalkerSummaryDto petWalker)
  {
    try
    {
      Logger.LogInformation("Pet walker selected: {PetWalkerId} - {PetWalkerName}", petWalker.Id, petWalker.FullName);

      // Toggle selection - if same pet walker is clicked, deselect
      if (selectedPetWalker?.Id == petWalker.Id)
      {
        selectedPetWalker = null;
      }
      else
      {
        selectedPetWalker = petWalker;
      }

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
      if (selectedPetWalker == null)
      {
        Logger.LogWarning("Attempted to proceed to booking without selected pet walker");
        return;
      }

      Logger.LogInformation("Proceeding to booking with pet walker: {PetWalkerId} - {PetWalkerName}", 
        selectedPetWalker.Id, selectedPetWalker.FullName);

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
