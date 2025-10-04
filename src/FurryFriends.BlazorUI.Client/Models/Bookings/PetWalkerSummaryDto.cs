using FurryFriends.BlazorUI.Client.Models.PetWalkers;

namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Summary information about a PetWalker for booking selection
/// </summary>
public class PetWalkerSummaryDto
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
  public PetWalkerBasicPhotoDto? BioPicture { get; set; }
  public double Rating { get; set; } = 0.0;
  public int ReviewCount { get; set; } = 0;
  public List<string> ServiceAreas { get; set; } = new();
}
