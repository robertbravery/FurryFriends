using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Update;

public class UpdateClientResponse
{
  public Guid ClientId { get; set; }
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Address { get; set; } = default!;
  public ClientType ClientType { get; set; }
  public TimeOnly? PreferredContactTime { get; set; }
  public ReferralSource? ReferralSource { get; set; }
}
