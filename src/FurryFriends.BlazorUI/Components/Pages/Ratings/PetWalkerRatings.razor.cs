using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Components.Pages.Ratings;

public partial class PetWalkerRatings
{
    [Inject]
    public IRatingService RatingService { get; set; } = default!;

    [Inject]
    public ILogger<PetWalkerRatings> Logger { get; set; } = default!;

    [Parameter]
    public Guid PetWalkerId { get; set; }

    [Parameter]
    public string HeaderText { get; set; } = "Ratings & Reviews";

    [Parameter]
    public int DefaultPageSize { get; set; } = 10;

    [Parameter]
    public EventCallback<int> OnTotalCountChanged { get; set; }

    public List<RatingDto>? Ratings { get; private set; }
    public int CurrentPage { get; private set; } = 1;
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage { get; private set; }
    public bool HasNextPage { get; private set; }
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        PageSize = DefaultPageSize;
        await LoadRatings();
    }

    public async Task LoadRatings()
    {
        if (PetWalkerId == Guid.Empty) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            StateHasChanged();

            Logger.LogInformation(
                "Loading ratings for PetWalker: {PetWalkerId}, Page: {Page}, PageSize: {PageSize}",
                PetWalkerId, CurrentPage, PageSize);

            var result = await RatingService.GetRatingsAsync(PetWalkerId, CurrentPage, PageSize);

            Ratings = result.Items;
            CurrentPage = result.PageNumber;
            PageSize = result.PageSize;
            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            HasPreviousPage = result.HasPreviousPage;
            HasNextPage = result.HasNextPage;

            Logger.LogInformation(
                "Loaded {Count} ratings for PetWalker: {PetWalkerId} (Page {Page}/{TotalPages})",
                Ratings.Count, PetWalkerId, CurrentPage, TotalPages);

            await OnTotalCountChanged.InvokeAsync(TotalCount);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading ratings for PetWalker: {PetWalkerId}", PetWalkerId);
            ErrorMessage = "Failed to load ratings. Please try again later.";
            Ratings = new List<RatingDto>();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    public async Task GoToPreviousPage()
    {
        if (!HasPreviousPage) return;
        CurrentPage--;
        await LoadRatings();
    }

    public async Task GoToNextPage()
    {
        if (!HasNextPage) return;
        CurrentPage++;
        await LoadRatings();
    }

    public async Task Refresh()
    {
        CurrentPage = 1;
        await LoadRatings();
    }

    public async Task ChangePageSize(int newPageSize)
    {
        PageSize = newPageSize;
        CurrentPage = 1;
        await LoadRatings();
    }
}
