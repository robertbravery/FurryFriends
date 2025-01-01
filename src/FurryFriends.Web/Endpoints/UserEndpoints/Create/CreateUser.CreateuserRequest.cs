using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Create;

public class CreateUserRequest
{
  public const string Route = "/Users";

  [Required(ErrorMessage = "First Name is required.")]
  public string FirstName { get; set; } = default!;

  [Required(ErrorMessage = "Last Name is required.")]
  public string LastName { get; set; } = default!;


  [Required(ErrorMessage = "Email is required.")]
  [EmailAddress(ErrorMessage = "Invalid email format.")]
  public string Email { get; set; } = default!;

  [Required(ErrorMessage = "Country code is required.")]
  public string CountryCode { get; set; } = default!;

  [Required(ErrorMessage = "Area code is required.")]
  [StringLength(3), RegularExpression(@"^\d{3}$", ErrorMessage = "Invalid area code format.")]
  public string AreaCode { get; set; } = default!;

  [Required(ErrorMessage = "Number is required.")]
  public string Number { get; set; } = default!;

  [Required(ErrorMessage = "Street is required.")]
  public string Street { get; set; } = default!;

  [Required(ErrorMessage = "City is required.")]
  public string City { get; set; } = default!;

  [Required(ErrorMessage = "State is required.")]
  public string State { get; set; } = default!;

  [Required(ErrorMessage = "Country is required.")]
  public string Country { get; set; } = default!;

  [Required(ErrorMessage = "Postal code is required.")]
  public string PostalCode { get; set; } = default!;
}
