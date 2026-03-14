namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// API response model for pet walker summary data
/// </summary>

public class PetWalkerSummaryApiResponse
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
