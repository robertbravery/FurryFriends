namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// API response model for GetAvailablePetWalkers endpoint
/// </summary>

public class GetAvailablePetWalkersApiResponse
{
    public List<PetWalkerSummaryApiResponse> PetWalkers { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public List<string> AvailableServiceAreas { get; set; } = new();
}
