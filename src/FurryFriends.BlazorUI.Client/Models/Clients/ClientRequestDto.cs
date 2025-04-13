namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientRequestDto
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string PhoneCountryCode { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;

  // Flattened Address fields
  public string Street { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public string ZipCode { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;

  public string Notes { get; set; } = string.Empty;
  public static ClientRequestDto MapToDto(ClientModel clientModel)
  {
    return new ClientRequestDto
    {
      FirstName = clientModel.FirstName,
      LastName = clientModel.LastName,
      Email = clientModel.EmailAddress,
      PhoneCountryCode = clientModel.CountryCode,
      PhoneNumber = clientModel.PhoneNumber,
      Street = clientModel.Address.Street,
      City = clientModel.Address.City,
      State = clientModel.Address.State,
      ZipCode = clientModel.Address.ZipCode,
      Country = clientModel.Address.Country,
      Notes = clientModel.Notes
    };
  }
}
