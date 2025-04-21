using System.ComponentModel.DataAnnotations;

namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientModel
{
  [Required(ErrorMessage = "Full name is required")]
  [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
  [RegularExpression(@"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$",
      ErrorMessage = "First Name can only contain letters, spaces, and characters: ' , . -")]
  public string FirstName { get; set; } = string.Empty;

  [Required(ErrorMessage = "Full name is required")]
  [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
  [RegularExpression(@"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$",
      ErrorMessage = "Last Name can only contain letters, spaces, and characters: ' , . -")]
  public string LastName { get; set; } = string.Empty;

  [Required(ErrorMessage = "Email address is required")]
  [EmailAddress(ErrorMessage = "Invalid email format")]
  [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
  public string EmailAddress { get; set; } = string.Empty;

  [Required(ErrorMessage = "Country code is required")]
  [RegularExpression(@"^[0-9]\d{1,3}$",
     ErrorMessage = "Please enter a valid country code")]
  public string CountryCode { get; set; } = string.Empty;

  [Required(ErrorMessage = "Phone number is required")]
  [RegularExpression(@"^[0-9]\d{1,14}$",
      ErrorMessage = "Please enter a valid phone number")]
  public string PhoneNumber { get; set; } = string.Empty;

  [ValidateComplexType]
  public Address Address { get; set; } = new Address();

  [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
  public string Notes { get; set; } = string.Empty;

  public static ClientRequestDto MapToRequest(ClientModel clientDto) => new()
  {
    FirstName = clientDto.FirstName,
    LastName = clientDto.LastName,
    Email = clientDto.EmailAddress,
    PhoneCountryCode = clientDto.CountryCode,
    PhoneNumber = clientDto.PhoneNumber,
    Street = clientDto.Address.Street,
    City = clientDto.Address.City,
    State = clientDto.Address.State,
    ZipCode = clientDto.Address.ZipCode,
    Country = clientDto.Address.Country,
    Notes = clientDto.Notes
  };
}
