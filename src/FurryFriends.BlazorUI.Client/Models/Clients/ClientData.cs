namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientData
{
  public Guid Id { get; set; }
  public string Name { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Street { get; set; } = default!;
  public string City { get; set; } = default!;
  public string State { get; set; } = default!;
  public string ZipCode { get; set; } = default!;
  public string Country { get; set; } = default!;
  public int ClientType { get; set; }
  public string PreferredContactTime { get; set; } = default!;
  public int ReferralSource { get; set; }
  public Pet[] Pets { get; set; } = default!;

  public static ClientModel MapToModel(ClientData clientData)
  {
    // Parse phone number to extract country code and number
    string countryCode = "1"; // Default to US
    string phoneNumber = clientData.PhoneNumber;

    // If phone number contains a plus sign, extract the country code
    if (clientData.PhoneNumber.StartsWith("+") && clientData.PhoneNumber.Length > 1)
    {
      var parts = clientData.PhoneNumber.TrimStart('+').Split(' ', 2);
      if (parts.Length > 1)
      {
        countryCode = parts[0];
        phoneNumber = parts[1];
      }
      else
      {
        // Try to extract first 1-3 digits as country code
        if (parts[0].Length > 3)
        {
          countryCode = parts[0].Substring(0, Math.Min(3, parts[0].Length));
          phoneNumber = parts[0].Substring(countryCode.Length);
        }
        else
        {
          phoneNumber = parts[0];
        }
      }
    }

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
      FirstName = firstName,
      LastName = lastName,
      EmailAddress = clientData.Email,
      CountryCode = countryCode,
      PhoneNumber = phoneNumber,
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
