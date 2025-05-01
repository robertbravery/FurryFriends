using System.Text.Json.Serialization;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class PetWalkerApiResponse
{
  [JsonPropertyName("id")]
  public Guid Id { get; set; }

  [JsonPropertyName("fullName")]
  public string FullName { get; set; } = string.Empty;

  [JsonPropertyName("email")]
  public string Email { get; set; } = string.Empty;

  [JsonPropertyName("countryCode")]
  public string CountryCode { get; set; } = string.Empty;

  [JsonPropertyName("phoneNumber")]
  public string PhoneNumber { get; set; } = string.Empty;

  [JsonPropertyName("city")]
  public string City { get; set; } = string.Empty;

  [JsonPropertyName("hourlyRate")]
  public decimal HourlyRate { get; set; } = 25.00m;

  [JsonPropertyName("currency")]
  public string Currency { get; set; } = "USD";

  [JsonPropertyName("yearsOfExperience")]
  public int YearsOfExperience { get; set; } = 1;

  [JsonPropertyName("dailyPetWalkLimit")]
  public int DailyPetWalkLimit { get; set; } = 5;

  [JsonPropertyName("isVerified")]
  public bool IsVerified { get; set; } = true;

  [JsonPropertyName("hasInsurance")]
  public bool HasInsurance { get; set; } = true;

  [JsonPropertyName("hasFirstAidCertification")]
  public bool HasFirstAidCertification { get; set; } = true;

  [JsonPropertyName("gender")]
  public string Gender { get; set; } = "Not specified";

  [JsonPropertyName("biography")]
  public string Biography { get; set; } = "This pet walker has not provided a biography yet.";

  [JsonPropertyName("locations")]
  public List<string> Locations { get; set; } = new();

  [JsonPropertyName("serviceLocation")]
  public List<string> ServiceLocation { get; set; } = new();

  [JsonPropertyName("street")]
  public string Street { get; set; } = string.Empty;

  [JsonPropertyName("state")]
  public string State { get; set; } = string.Empty;

  [JsonPropertyName("zipCode")]
  public string ZipCode { get; set; } = string.Empty;

  [JsonPropertyName("country")]
  public string Country { get; set; } = string.Empty;

  [JsonPropertyName("bioPicture")]
  public PetWalkerBasicPhotoDto? BioPicture { get; set; }

  [JsonPropertyName("photos")]
  public List<PetWalkerBasicPhotoDto> Photos { get; set; } = new();
}
