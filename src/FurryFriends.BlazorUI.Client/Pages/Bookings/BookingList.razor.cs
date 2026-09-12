using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Pages.Bookings;

public partial class BookingList
{
  [Inject] public ILogger<BookingList> Logger { get; set; } = default!;
  [Inject] public IBookingService BookingService { get; set; } = default!;
  [Inject] public IClientService ClientService { get; set; } = default!;
  [Inject] public IPetWalkerService PetWalkerService { get; set; } = default!;
  [Inject] public NavigationManager Navigation { get; set; } = default!;
  [Inject] public IClientContextService ClientContextService { get; set; } = default!;

  [Parameter] public Guid? ClientId { get; set; }
  [Parameter] public Guid? PetWalkerId { get; set; }

  private BookingViewMode _viewMode = BookingViewMode.Client;
  private Guid _selectedIdentityId = Guid.Empty;
  private DateTime _startDate = DateTime.Today.AddDays(-30);
  private DateTime _endDate = DateTime.Today.AddDays(30);

  private readonly List<BookingListItemDto> _bookings = new();
  private readonly List<IdentityOption> _identities = new();
  private readonly HashSet<Guid> _processingBookingIds = new();

  private bool _isLoading;
  private bool _isSubmitting;
  private string? _errorMessage;
  private string? _successMessage;

  private BookingListItemDto? _pendingBooking;
  private BookingAction? _pendingAction;
  private BookingListItemDto? _bookingToCancel;
  private CancellationReason _cancelReason = CancellationReason.ClientRequest;
  private string? _cancelNotes;

  private Guid SelectedIdentityId => _selectedIdentityId;
  private bool HasIdentities => _identities.Count > 0;
  private bool HasInvalidDateRange => _startDate.Date > _endDate.Date;
  private bool CanLoad => SelectedIdentityId != Guid.Empty && !HasInvalidDateRange && !_isLoading;

  /// <summary>
  /// How many clients/pet walkers to pull into the identity pickers. These are lookup lists
  /// rather than paged grids, so the limit has to be generous enough that the signed-in client
  /// is actually present - a truncated list makes them impossible to select.
  /// </summary>
  private const int IdentityPageSize = 200;

  private string IdentityLabel => _viewMode == BookingViewMode.Client ? "Client" : "Pet Walker";
  private string IdentityLabelLower => _viewMode == BookingViewMode.Client ? "client" : "pet walker";
  private string IdentityPromptTitle => $"Select a {IdentityLabel}";
  private string IdentityPromptDescription =>
    "Choose one above to see their bookings and manage their progress.";

  protected override async Task OnInitializedAsync()
  {
    Logger.LogInformation(
      "BookingList initialized. ClientId: {ClientId}, PetWalkerId: {PetWalkerId}",
      ClientId, PetWalkerId);

    if (PetWalkerId.HasValue)
    {
      _viewMode = BookingViewMode.PetWalker;
      _selectedIdentityId = PetWalkerId.Value;
    }
    else if (ClientId.HasValue)
    {
      _viewMode = BookingViewMode.Client;
      _selectedIdentityId = ClientId.Value;
    }
    else
    {
      // Default the Client view to the signed-in client. Their own bookings are what they came
      // here for, and it saves them hunting for themselves in the picker.
      _viewMode = BookingViewMode.Client;
      _selectedIdentityId = await ResolveCurrentClientIdAsync();

      Logger.LogInformation("BookingList defaulted the Client view to the current client: {ClientId}",
        _selectedIdentityId);
    }

    await LoadIdentitiesAsync();

    if (_selectedIdentityId != Guid.Empty)
    {
      await LoadBookingsAsync();
    }
  }

  /// <summary>
  /// The signed-in client, or <see cref="Guid.Empty"/> when no client context is available
  /// (in which case the user simply picks one themselves).
  /// </summary>
  private async Task<Guid> ResolveCurrentClientIdAsync()
  {
    try
    {
      return await ClientContextService.GetCurrentClientIdAsync();
    }
    catch (Exception ex)
    {
      Logger.LogWarning(ex, "Could not resolve the current client for the bookings list");
      return Guid.Empty;
    }
  }

  private async Task SetViewModeAsync(BookingViewMode mode)
  {
    if (_viewMode == mode)
    {
      return;
    }

    _viewMode = mode;
    _selectedIdentityId = Guid.Empty;
    _bookings.Clear();
    _errorMessage = null;
    _successMessage = null;

    await LoadIdentitiesAsync();

    // Switching to the Client view gets the same courtesy as loading the page: default to the
    // signed-in client rather than leaving the picker empty.
    if (mode == BookingViewMode.Client)
    {
      _selectedIdentityId = await ResolveCurrentClientIdAsync();

      if (_selectedIdentityId != Guid.Empty)
      {
        await LoadBookingsAsync();
      }
    }
  }

  private async Task LoadIdentitiesAsync()
  {
    _identities.Clear();

    try
    {
      if (_viewMode == BookingViewMode.Client)
      {
        // Note: the parameterless GetClientsAsync overload caps the result at 10 clients, which
        // silently omitted the signed-in client from the picker. Use the paged overload instead.
        var clients = await ClientService.GetClientsAsync(1, IdentityPageSize);

        if (clients?.RowsData is { } rows)
        {
          _identities.AddRange(rows
            .Where(c => c.Id != Guid.Empty)
            .Select(c => new IdentityOption(c.Id, string.IsNullOrWhiteSpace(c.FullName) ? c.EmailAddress ?? "Unnamed client" : c.FullName!)));
        }
      }
      else
      {
        var response = await PetWalkerService.GetPetWalkersAsync(1, IdentityPageSize);
        _identities.AddRange((response.RowsData ?? new List<Models.PetWalkers.PetWalkerDto>())
          .Where(p => p.Id != Guid.Empty)
          .Select(p => new IdentityOption(p.Id, string.IsNullOrWhiteSpace(p.Name) ? p.EmailAddress : p.Name)));
      }

      Logger.LogInformation("Loaded {Count} {Mode} options for booking management", _identities.Count, _viewMode);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Failed to load identity options for booking management");
      _errorMessage = "Could not load the list of " + (_viewMode == BookingViewMode.Client ? "clients." : "pet walkers.");
    }
  }

  private async Task OnIdentityChangedAsync(ChangeEventArgs args)
  {
    var value = args.Value?.ToString();

    _selectedIdentityId = Guid.TryParse(value, out var id) ? id : Guid.Empty;
    _successMessage = null;
    _errorMessage = null;
    _bookings.Clear();

    if (_selectedIdentityId != Guid.Empty)
    {
      await LoadBookingsAsync();
    }
  }

  private async Task OnStartDateChangedAsync(ChangeEventArgs args)
  {
    if (DateTime.TryParse(args.Value?.ToString(), out var date))
    {
      _startDate = date;
      await ReloadIfPossibleAsync();
    }
  }

  private async Task OnEndDateChangedAsync(ChangeEventArgs args)
  {
    if (DateTime.TryParse(args.Value?.ToString(), out var date))
    {
      _endDate = date;
      await ReloadIfPossibleAsync();
    }
  }

  private async Task ReloadIfPossibleAsync()
  {
    if (SelectedIdentityId != Guid.Empty && !HasInvalidDateRange)
    {
      await LoadBookingsAsync();
    }
  }

  private async Task LoadBookingsAsync()
  {
    if (SelectedIdentityId == Guid.Empty)
    {
      return;
    }

    if (HasInvalidDateRange)
    {
      _errorMessage = "The \"From\" date must be on or before the \"To\" date.";
      return;
    }

    _isLoading = true;
    _errorMessage = null;

    try
    {
      var response = _viewMode == BookingViewMode.Client
        ? await BookingService.GetClientBookingsAsync(SelectedIdentityId, _startDate, _endDate)
        : await BookingService.GetPetWalkerBookingsAsync(SelectedIdentityId, _startDate, _endDate);

      _bookings.Clear();

      if (response.Success && response.Data != null)
      {
        _bookings.AddRange(response.Data.OrderBy(b => b.StartTime));
        Logger.LogInformation("Loaded {Count} bookings for {Mode} {Id}", _bookings.Count, _viewMode, SelectedIdentityId);
      }
      else
      {
        _errorMessage = BuildErrorMessage(response.Errors, response.Message, "Failed to load bookings.");
        Logger.LogWarning("Failed to load bookings for {Mode} {Id}: {Error}", _viewMode, SelectedIdentityId, _errorMessage);
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error loading bookings for {Mode} {Id}", _viewMode, SelectedIdentityId);
      _errorMessage = "An unexpected error occurred while loading bookings.";
    }
    finally
    {
      _isLoading = false;
      StateHasChanged();
    }
  }

  private void GoToNewBooking() => Navigation.NavigateTo("/bookings/new");

  private void ClearSuccess() => _successMessage = null;

  // ---------- Lifecycle actions ----------

  private void RequestActionAsync(BookingListItemDto booking, BookingAction action)
  {
    _pendingBooking = booking;
    _pendingAction = action;
    _successMessage = null;
  }

  private void CloseConfirmModal()
  {
    if (_isSubmitting)
    {
      return;
    }

    _pendingBooking = null;
    _pendingAction = null;
  }

  private async Task ExecutePendingActionAsync()
  {
    if (_pendingBooking == null || _pendingAction == null)
    {
      return;
    }

    var booking = _pendingBooking;
    var action = _pendingAction.Value;

    _isSubmitting = true;
    _processingBookingIds.Add(booking.Id);
    _errorMessage = null;

    try
    {
      var response = action switch
      {
        BookingAction.Confirm => await BookingService.ConfirmBookingAsync(booking.Id),
        BookingAction.Start => await BookingService.StartBookingAsync(booking.Id),
        BookingAction.Complete => await BookingService.CompleteBookingAsync(booking.Id),
        BookingAction.NoShow => await BookingService.MarkBookingNoShowAsync(booking.Id),
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
      };

      if (response.Success)
      {
        _successMessage = response.Message;
        _pendingBooking = null;
        _pendingAction = null;
        await LoadBookingsAsync();
      }
      else
      {
        _errorMessage = BuildErrorMessage(response.Errors, response.Message, "The booking could not be updated.");
        _pendingBooking = null;
        _pendingAction = null;
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error performing {Action} on booking {BookingId}", action, booking.Id);
      _errorMessage = "An unexpected error occurred while updating the booking.";
      _pendingBooking = null;
      _pendingAction = null;
    }
    finally
    {
      _isSubmitting = false;
      _processingBookingIds.Remove(booking.Id);
      StateHasChanged();
    }
  }

  private void OpenCancelModal(BookingListItemDto booking)
  {
    _bookingToCancel = booking;
    _cancelReason = CancellationReason.ClientRequest;
    _cancelNotes = null;
    _successMessage = null;
  }

  private void CloseCancelModal()
  {
    if (_isSubmitting)
    {
      return;
    }

    _bookingToCancel = null;
  }

  private async Task ConfirmCancellationAsync()
  {
    if (_bookingToCancel == null)
    {
      return;
    }

    var booking = _bookingToCancel;

    _isSubmitting = true;
    _processingBookingIds.Add(booking.Id);
    _errorMessage = null;

    try
    {
      var request = new CancelBookingRequestDto
      {
        Reason = _cancelReason,
        CancelledBy = _viewMode == BookingViewMode.Client ? CancelledBy.Client : CancelledBy.PetWalker,
        AdditionalNotes = string.IsNullOrWhiteSpace(_cancelNotes) ? null : _cancelNotes
      };

      var response = await BookingService.CancelBookingAsync(booking.Id, request);

      if (response.Success)
      {
        _successMessage = response.Data?.Message ?? "Booking cancelled successfully.";
        _bookingToCancel = null;
        await LoadBookingsAsync();
      }
      else
      {
        _errorMessage = BuildErrorMessage(response.Errors, response.Message, "The booking could not be cancelled.");
        _bookingToCancel = null;
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error cancelling booking {BookingId}", booking.Id);
      _errorMessage = "An unexpected error occurred while cancelling the booking.";
      _bookingToCancel = null;
    }
    finally
    {
      _isSubmitting = false;
      _processingBookingIds.Remove(booking.Id);
      StateHasChanged();
    }
  }

  private bool IsProcessing(Guid bookingId) => _processingBookingIds.Contains(bookingId);

  // ---------- Status rules are owned by BookingStatusRules (mirrors the API/domain state machine) ----------

  private static bool CanConfirm(string? status) => BookingStatusRules.CanConfirm(status);
  private static bool CanStart(string? status) => BookingStatusRules.CanStart(status);
  private static bool CanComplete(string? status) => BookingStatusRules.CanComplete(status);
  private static bool CanMarkNoShow(string? status) => BookingStatusRules.CanMarkNoShow(status);
  private static bool CanCancel(string? status) => BookingStatusRules.CanCancel(status);
  private static bool HasAnyAction(string? status) => BookingStatusRules.HasAnyAction(status);
  private static string StatusChipClass(string? status) => BookingStatusRules.StatusChipClass(status);

  // ---------- Display helpers ----------

  private static string FormatDate(DateTime value) => value.ToString("ddd, dd MMM yyyy");

  private static string FormatTimeRange(BookingListItemDto booking) =>
    $"{booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}";

  private static string FormatPrice(decimal price) => $"${price:F2}";

  private static string BuildErrorMessage(List<string>? errors, string? fallback, string defaultMessage)
  {
    if (errors != null && errors.Count > 0)
    {
      return string.Join(" ", errors);
    }

    return string.IsNullOrWhiteSpace(fallback) ? defaultMessage : fallback;
  }

  private static string ActionTitle(BookingAction action) => action switch
  {
    BookingAction.Confirm => "Confirm this booking?",
    BookingAction.Start => "Start this walk?",
    BookingAction.Complete => "Complete this walk?",
    BookingAction.NoShow => "Mark as no-show?",
    _ => "Update this booking?"
  };

  private static string ActionQuestion(BookingAction action) => action switch
  {
    BookingAction.Confirm => "The booking will move to Confirmed and the pet walker will be expected to attend.",
    BookingAction.Start => "The booking will move to InProgress.",
    BookingAction.Complete => "The booking will move to Completed. This cannot be undone.",
    BookingAction.NoShow => "The booking will be recorded as a no-show. This cannot be undone.",
    _ => string.Empty
  };

  private static string ActionConfirmLabel(BookingAction action) => action switch
  {
    BookingAction.Confirm => "Confirm Booking",
    BookingAction.Start => "Start Booking",
    BookingAction.Complete => "Complete Booking",
    BookingAction.NoShow => "Mark No-show",
    _ => "Confirm"
  };

  private static string ActionIcon(BookingAction action) => action switch
  {
    BookingAction.Confirm => "fa-circle-check",
    BookingAction.Start => "fa-person-walking",
    BookingAction.Complete => "fa-flag-checkered",
    BookingAction.NoShow => "fa-user-slash",
    _ => "fa-pen"
  };

  private record IdentityOption(Guid Id, string Name);

  private enum BookingViewMode
  {
    Client,
    PetWalker
  }

  private enum BookingAction
  {
    Confirm,
    Start,
    Complete,
    NoShow
  }
}
