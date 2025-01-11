using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Create;

public class CreateClientRequest
{

  public const string Route = "/Clients";

  public string FirstName { get; set; } = default!;

  public string LastName { get; set; } = default!;

  public string Email { get; set; } = default!;
  public string CountryCode { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Street { get; set; } = default!;
  public string City { get; set; } = default!;
  public string State { get; set; } = default!;
  public string Country { get; set; } = default!;
  public string ZipCode { get; set; } = default!;
  public ClientType ClientType { get; set; } = ClientType.Regular!;
  public TimeOnly? PreferredContactTime { get; set; } = default!;
  public string? ReferralSource { get; set; } = default!;
}
