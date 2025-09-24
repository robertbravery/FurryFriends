using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Pages.Clients;

public partial class ClientList : IDisposable
{
  public List<ClientDto>? clients { get; set; }
  public int currentPage = 1;
  public int pageSize = 10;
  public int totalCount = 0;
  public int totalPages = 1;
  public bool hasPreviousPage = false;
  public bool hasNextPage = false;
  public bool isLoading = false;
  public string? errorMessage = null;

  [Inject]
  public IClientService ClientService { get; set; } = default!;

  [Inject]
  public IPopupService PopupService { get; set; } = default!;

  [Inject]
  public ILogger<ClientList> Logger { get; set; } = default!;

  protected override async Task OnInitializedAsync()
  {
    await LoadClients();

    // Subscribe to the popup close event to refresh data
    SubscribeToEvents();
  }

  protected override void OnAfterRender(bool firstRender)
  {
    if (firstRender)
    {
      // Ensure we're subscribed after the component is rendered
      SubscribeToEvents();
    }
  }

  public void SubscribeToEvents()
  {
    // Unsubscribe first to avoid duplicate subscriptions
    PopupService.OnCloseEditClientPopup -= HandlePopupClosed;
    PopupService.OnCloseViewClientPopup -= HandlePopupClosed;
    PopupService.OnCloseCreateClientPopup -= HandlePopupClosed;

    // Subscribe to events
    PopupService.OnCloseEditClientPopup += HandlePopupClosed;
    PopupService.OnCloseViewClientPopup += HandlePopupClosed;
    PopupService.OnCloseCreateClientPopup += HandlePopupClosed;
  }

  public async Task LoadClients()
  {
    try
    {
      isLoading = true;
      errorMessage = null;

      Logger.LogInformation("Loading clients - Page: {CurrentPage}, PageSize: {PageSize}", currentPage, pageSize);
      var response = await ClientService.GetClientsAsync(currentPage, pageSize);

      // Update the component state with data from the response
      clients = response.RowsData;
      totalCount = response.TotalCount;
      totalPages = response.TotalPages;
      hasPreviousPage = response.HasPreviousPage;
      hasNextPage = response.HasNextPage;

      // Log pagination values
      Logger.LogDebug("Pagination - TotalCount: {TotalCount}, PageSize: {PageSize}, TotalPages: {TotalPages}",
          totalCount, pageSize, totalPages);
      Logger.LogDebug("Pagination - HasPreviousPage: {HasPreviousPage}, HasNextPage: {HasNextPage}, CurrentPage: {CurrentPage}",
          hasPreviousPage, hasNextPage, currentPage);

      // Use the page number from the response (in case it was adjusted on the server)
      currentPage = response.PageNumber;

      // Ensure current page is valid
      if (currentPage > totalPages && totalPages > 0)
      {
        Logger.LogWarning("Current page {CurrentPage} exceeds total pages {TotalPages}, adjusting to last page",
            currentPage, totalPages);
        currentPage = totalPages;
        await LoadClients();
      }

      Logger.LogInformation("Successfully loaded {Count} clients", clients?.Count);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error loading clients");
      errorMessage = $"Error loading clients: {ex.Message}";
      clients = new List<ClientDto>(); // Empty list to avoid null reference exceptions
    }
    finally
    {
      isLoading = false;
      StateHasChanged();
    }
  }

  public async void HandlePopupClosed()
  {
    // Refresh the client list when the popup is closed
    await LoadClients();
    StateHasChanged();
  }

  public void OpenEditPopup(string? clientEmail)
  {
    if (clientEmail is null)
    {
      Logger.LogWarning("Attempted to open edit popup with null client email");
      return;
    }
    Logger.LogInformation("Opening edit popup for client email: {ClientEmail}", clientEmail);

    // Use the popup service to show the edit popup
    PopupService.ShowEditClientPopup(clientEmail);
  }

  public void OpenViewPopup(string? clientEmail)
  {
    if (clientEmail is null)
    {
      Logger.LogWarning("Attempted to open view popup with null client email");
      return;
    }
    Logger.LogInformation("Opening view popup for client email: {ClientEmail}", clientEmail);

    // Use the popup service to show the view popup
    PopupService.ShowViewClientPopup(clientEmail);
  }

  public void OpenCreatePopup()
  {
    Logger.LogInformation("Opening create client popup");

    // Use the popup service to show the create popup
    PopupService.ShowCreateClientPopup();
  }

  public async Task HandlePageChanged(int newPage)
  {
    Logger.LogInformation("Changing page from {OldPage} to {NewPage}", currentPage, newPage);
    currentPage = newPage;
    await LoadClients();
  }

  public async Task HandlePageSizeChanged(int newPageSize)
  {
    Logger.LogInformation("Changing page size from {OldPageSize} to {NewPageSize}", pageSize, newPageSize);
    pageSize = newPageSize;
    await LoadClients();
  }

  public void Dispose()
  {
    try
    {
      Logger.LogDebug("Disposing ClientList component and unsubscribing from events");
      // Unsubscribe from events when the component is disposed
      PopupService.OnCloseEditClientPopup -= HandlePopupClosed;
      PopupService.OnCloseViewClientPopup -= HandlePopupClosed;
      PopupService.OnCloseCreateClientPopup -= HandlePopupClosed;
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error during ClientList component disposal");
    }
  }
}
