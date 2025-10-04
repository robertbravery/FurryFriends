namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Paginated response for available PetWalkers
/// </summary>
public class PaginatedPetWalkersResponse
{
    public List<PetWalkerSummaryDto> PetWalkers { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public List<string> AvailableServiceAreas { get; set; } = new();
}

/// <summary>
/// Request parameters for getting available PetWalkers
/// </summary>
public class GetAvailablePetWalkersRequest
{
    // Filtering
    public string ServiceArea { get; set; } = string.Empty;
    public string SearchTerm { get; set; } = string.Empty;
    
    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    
    // Sorting
    public string SortBy { get; set; } = "name"; // name, rate, experience, rating
    public string SortDirection { get; set; } = "asc"; // asc, desc
}
