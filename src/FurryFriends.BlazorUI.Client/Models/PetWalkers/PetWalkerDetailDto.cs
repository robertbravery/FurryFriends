using FurryFriends.BlazorUI.Client.Models.Locations;

namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

public class PetWalkerDetailDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string EmailAddress { get; set; } = string.Empty;
  public string CountryCode { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string Street { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public string ZipCode { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;
  public string Biography { get; set; } = string.Empty;
  public DateTime DateOfBirth { get; set; }
  public string Gender { get; set; } = string.Empty;
  public decimal HourlyRate { get; set; }
  public string Currency { get; set; } = string.Empty;
  public bool IsActive { get; set; }
  public bool IsVerified { get; set; }
  public int YearsOfExperience { get; set; }
  public bool HasInsurance { get; set; }
  public bool HasFirstAidCertification { get; set; }
  public int DailyPetWalkLimit { get; set; }

  // Legacy service areas as strings (for backward compatibility)
  public List<string> ServiceAreas { get; set; } = new List<string>();

  // New service areas as structured objects
  public List<ServiceAreaDto> StructuredServiceAreas { get; set; } = new List<ServiceAreaDto>();

  public PetWalkerBasicPhotoDto? ProfilePicture { get; set; }
  public List<PetWalkerBasicPhotoDto> Photos { get; set; } = new List<PetWalkerBasicPhotoDto>();
}
