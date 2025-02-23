using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;
using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Command.UpdateClient;

public class UpdateClientCommand : IRequest<Client>
{
  public Guid ClientId { get; set; }
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string CountryCode { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Street { get; set; } = default!;
  public string City { get; set; } = default!;
  public string StateProvinceRegion { get; set; } = default!;
  public string ZipCode { get; set; } = default!;
  public string Country { get; set; } = default!;
  public ClientType ClientType { get; set; }
  public TimeOnly? PreferredContactTime { get; set; }
  public string? ReferralSource { get; set; }
}
