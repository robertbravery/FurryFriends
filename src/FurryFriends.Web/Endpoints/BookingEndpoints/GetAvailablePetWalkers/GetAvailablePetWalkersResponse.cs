namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailablePetWalkers;

/// <summary>
/// Summary information about a PetWalker for booking selection
/// </summary>
public class PetWalkerSummaryResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public decimal HourlyRate { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public int YearsOfExperience { get; set; }
    public bool HasInsurance { get; set; }
    public bool HasFirstAidCertification { get; set; }
    public int DailyPetWalkLimit { get; set; }
    public string? BioPictureUrl { get; set; }
    public double Rating { get; set; } = 0.0;
    public int ReviewCount { get; set; } = 0;
    public List<string> ServiceAreas { get; set; } = new();
}

/// <summary>
/// Paginated response for available PetWalkers
/// </summary>
public class GetAvailablePetWalkersResponse
{
    public List<PetWalkerSummaryResponse> PetWalkers { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public List<string> AvailableServiceAreas { get; set; } = new();
}
