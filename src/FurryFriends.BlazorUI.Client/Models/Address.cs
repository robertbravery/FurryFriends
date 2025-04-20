using System.ComponentModel.DataAnnotations;

namespace FurryFriends.BlazorUI.Client.Models;

public class Address
{
  [Required(ErrorMessage = "Street is required")]
  public string Street { get; set; } = default!;

  [Required(ErrorMessage = "City is required")]
  public string City { get; set; } = default!;

  [Required(ErrorMessage = "State or Province is required")]
  public string State { get; set; } = default!;

  [Required(ErrorMessage = "Zip Code is required")]
  [RegularExpression(@"^\d{4}(-\d{4})?$", ErrorMessage = "Invalid Zip Code")]
  public string ZipCode { get; set; } = default!;

  [Required(ErrorMessage = "Country is required")]
  public string Country { get; set; } = default!;
}
