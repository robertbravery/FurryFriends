namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientData
{
  public Guid Id { get; set; }
  public string Name { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string PhoneCountryCode { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Street { get; set; } = default!;
  public string City { get; set; } = default!;
  public string State { get; set; } = default!;
  public string ZipCode { get; set; } = default!;
  public string Country { get; set; } = default!;
  public int ClientType { get; set; }
  public string PreferredContactTime { get; set; } = default!;
  public int ReferralSource { get; set; }
  public PetDto[] Pets { get; set; } = default!;

  public static ClientModel MapToModel(ClientData clientData)
  {

    // Parse name into first and last name
    string firstName = clientData.Name;
    string lastName = string.Empty;
    var nameParts = clientData.Name.Split(' ', 2);
    if (nameParts.Length > 1)
    {
      firstName = nameParts[0];
      lastName = nameParts[1];
    }

    // Parse preferred contact time
    TimeOnly? preferredTime = null;
    if (!string.IsNullOrEmpty(clientData.PreferredContactTime))
    {
      if (TimeOnly.TryParse(clientData.PreferredContactTime, out var time))
      {
        preferredTime = time;
      }
    }

    return new ClientModel
    {
      Id = clientData.Id,
      FirstName = firstName,
      LastName = lastName,
      EmailAddress = clientData.Email,
      CountryCode = clientData.PhoneCountryCode,
      PhoneNumber = clientData.PhoneNumber,
      Address = new Address
      {
        Street = clientData.Street,
        City = clientData.City,
        State = clientData.State,
        ZipCode = clientData.ZipCode,
        Country = clientData.Country,
      },
      ClientType = (Enums.ClientType)clientData.ClientType,
      PreferredContactTime = clientData.PreferredContactTime,
      ReferralSource = (Enums.ReferralSource)clientData.ReferralSource,
      Notes = string.Empty // API doesn't seem to return notes
    };
  }
}
