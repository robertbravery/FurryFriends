# Part 2: Blazor Mastery

This section covers advanced Blazor techniques using practical examples from the FurryFriends application.

## 1. Advanced Component Design

### 1.1 Component Inheritance and Composition
```csharp
// Base component with common functionality
public abstract class PetComponentBase : ComponentBase
{
    [Inject] protected IMediator Mediator { get; set; } = default!;
    [Inject] protected ILogger<PetComponentBase> Logger { get; set; } = default!;

    protected async Task<Result<T>> ExecuteWithLogging<T>(Func<Task<Result<T>>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing action");
            return Result.Error<T>("An unexpected error occurred");
        }
    }
}

// Concrete component implementation
public class PetList : PetComponentBase
{
    [Parameter] public Guid ClientId { get; set; }
    private IReadOnlyList<PetDto> _pets = Array.Empty<PetDto>();

    protected override async Task OnInitializedAsync()
    {
        var result = await ExecuteWithLogging(async () =>
            await Mediator.Send(new GetClientPetsQuery(ClientId)));

        if (result.IsSuccess)
            _pets = result.Value;
    }
}
```

### 1.2 Generic Components
```csharp
public class DataLoader<TItem> : ComponentBase
{
    [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; } = default!;
    [Parameter] public RenderFragment LoadingTemplate { get; set; } = default!;
    [Parameter] public RenderFragment ErrorTemplate { get; set; } = default!;
    [Parameter] public Func<Task<Result<TItem>>> LoadData { get; set; } = default!;

    private TItem? _data;
    private bool _isLoading = true;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _isLoading = true;
            var result = await LoadData();
            if (result.IsSuccess)
                _data = result.Value;
            else
                _error = result.Error;
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _isLoading = false;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (_isLoading)
        {
            builder.AddContent(0, LoadingTemplate);
            return;
        }

        if (_error != null)
        {
            builder.AddContent(1, ErrorTemplate);
            return;
        }

        if (_data != null)
        {
            builder.AddContent(2, ItemTemplate(_data));
        }
    }
}

// Usage
<DataLoader TItem="PetWalkerDto" LoadData="@(async () => await GetPetWalker(Id))">
    <LoadingTemplate>
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </LoadingTemplate>
    <ItemTemplate Context="petWalker">
        <div class="card">
            <div class="card-body">
                <h5 class="card-title">@petWalker.Name</h5>
                <p class="card-text">@petWalker.Description</p>
            </div>
        </div>
    </ItemTemplate>
    <ErrorTemplate>
        <div class="alert alert-danger">
            Failed to load pet walker details.
        </div>
    </ErrorTemplate>
</DataLoader>
```

## 2. State Management

### 2.1 Advanced State Container
```csharp
public class BookingState
{
    private readonly List<Booking> _bookings = new();
    public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

    private readonly Dictionary<Guid, BookingStatus> _bookingStatuses = new();
    public IReadOnlyDictionary<Guid, BookingStatus> BookingStatuses => _bookingStatuses;

    public event Action? OnChange;

    public async Task AddBookingAsync(Booking booking)
    {
        _bookings.Add(booking);
        _bookingStatuses[booking.Id] = BookingStatus.Pending;
        NotifyStateChanged();
    }

    public void UpdateBookingStatus(Guid bookingId, BookingStatus status)
    {
        if (_bookingStatuses.ContainsKey(bookingId))
        {
            _bookingStatuses[bookingId] = status;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

// Service registration
services.AddScoped<BookingState>();

// Usage in component
@inject BookingState BookingState
@implements IDisposable

<div class="booking-list">
    @foreach (var booking in BookingState.Bookings)
    {
        <BookingCard Booking="@booking" 
                     Status="@BookingState.BookingStatuses[booking.Id]" />
    }
</div>

@code {
    protected override void OnInitialized()
    {
        BookingState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        BookingState.OnChange -= StateHasChanged;
    }
}
```

## 3. Real-time Updates with SignalR

### 3.1 Hub Implementation
```csharp
public class BookingHub : Hub
{
    public async Task JoinBookingGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveBookingGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task UpdateBookingStatus(Guid bookingId, BookingStatus status)
    {
        await Clients.Group($"booking_{bookingId}").SendAsync(
            "BookingStatusUpdated", 
            bookingId, 
            status);
    }
}
```

### 3.2 Blazor Integration
```csharp
@inject NavigationManager NavigationManager
@implements IAsyncDisposable

@code {
    private HubConnection? _hubConnection;
    private readonly string _bookingGroup;

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/bookinghub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<Guid, BookingStatus>(
            "BookingStatusUpdated",
            (bookingId, status) =>
            {
                BookingState.UpdateBookingStatus(bookingId, status);
                StateHasChanged();
            });

        await _hubConnection.StartAsync();
        await _hubConnection.SendAsync("JoinBookingGroup", _bookingGroup);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("LeaveBookingGroup", _bookingGroup);
            await _hubConnection.DisposeAsync();
        }
    }
}
```

## 4. JavaScript Interop

### 4.1 Map Integration Service
```csharp
public interface IMapService
{
    ValueTask InitializeMapAsync(string elementId, double lat, double lng);
    ValueTask AddMarkerAsync(double lat, double lng, string title);
    ValueTask SetCenterAsync(double lat, double lng);
}

public class MapService : IMapService
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public MapService(IJSRuntime js)
    {
        _js = js;
    }

    private async Task EnsureModuleAsync()
    {
        _module ??= await _js.InvokeAsync<IJSObjectReference>(
            "import", "./js/mapService.js");
    }

    public async ValueTask InitializeMapAsync(string elementId, double lat, double lng)
    {
        await EnsureModuleAsync();
        await _module!.InvokeVoidAsync("initializeMap", elementId, lat, lng);
    }

    public async ValueTask AddMarkerAsync(double lat, double lng, string title)
    {
        await EnsureModuleAsync();
        await _module!.InvokeVoidAsync("addMarker", lat, lng, title);
    }

    public async ValueTask SetCenterAsync(double lat, double lng)
    {
        await EnsureModuleAsync();
        await _module!.InvokeVoidAsync("setCenter", lat, lng);
    }
}
```

### 4.2 JavaScript Module
```javascript
// wwwroot/js/mapService.js
let map;

export function initializeMap(elementId, lat, lng) {
    map = new google.maps.Map(document.getElementById(elementId), {
        center: { lat, lng },
        zoom: 12
    });
}

export function addMarker(lat, lng, title) {
    new google.maps.Marker({
        position: { lat, lng },
        map,
        title
    });
}

export function setCenter(lat, lng) {
    map.setCenter({ lat, lng });
}
```

### 4.3 Component Usage
```csharp
@inject IMapService MapService
@implements IAsyncDisposable

<div id="map" style="height: 400px; width: 100%;"></div>

@code {
    [Parameter] public List<PetWalkerLocation> Locations { get; set; } = new();
    private ElementReference _mapElement;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await MapService.InitializeMapAsync("map", -26.2041, 28.0473);
            
            foreach (var location in Locations)
            {
                await MapService.AddMarkerAsync(
                    location.Latitude,
                    location.Longitude,
                    location.Name);
            }
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        // Cleanup if needed
    }
}
```

## Best Practices

1. **Component Design**
   - Use inheritance for common functionality
   - Create reusable generic components
   - Implement proper component lifecycle
   - Handle component disposal correctly

2. **State Management**
   - Keep state immutable
   - Use events for state updates
   - Implement proper scoping
   - Handle cleanup in Dispose methods

3. **Real-time Updates**
   - Implement reconnection logic
   - Use groups for targeted updates
   - Handle connection errors
   - Clean up connections properly

4. **JavaScript Interop**
   - Use JavaScript modules
   - Implement proper cleanup
   - Handle loading states
   - Cache module references

## Exercises

1. Create a generic data grid component with sorting and filtering
2. Implement real-time chat between client and pet walker
3. Add map clustering for multiple pet walker locations
4. Create a state management solution for complex forms

## Common Pitfalls

1. Not handling component disposal properly
2. Inefficient state management causing unnecessary renders
3. Memory leaks in real-time connections
4. Poor error handling in JavaScript interop

## Additional Resources

1. [Blazor Component Documentation](https://docs.microsoft.com/aspnet/core/blazor/components/)
2. [SignalR Documentation](https://docs.microsoft.com/aspnet/core/signalr/introduction)
3. [JavaScript Interop Guide](https://docs.microsoft.com/aspnet/core/blazor/javascript-interoperability/)

Next section: Advanced Backend Features