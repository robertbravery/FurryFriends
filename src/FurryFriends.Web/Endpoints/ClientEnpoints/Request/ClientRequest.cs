using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Request;

public class ClientRequest
{
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string PhoneCountryCode { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Street { get; set; } = default!;
  public string City { get; set; } = default!;
  public string State { get; set; } = default!;
  public string Country { get; set; } = default!; //ToDo: Use the Country Lookup Table and add the CountryId
  public string ZipCode { get; set; } = default!;
  public ClientType ClientType { get; set; } = ClientType.Regular!;
  public TimeOnly? PreferredContactTime { get; set; } = default!;
  public ReferralSource ReferralSource { get; set; } = ReferralSource.None!;
}
