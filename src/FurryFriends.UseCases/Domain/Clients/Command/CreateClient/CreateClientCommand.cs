using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.UseCases.Domain.Clients.Command.CreateClient;
public record CreateClientCommand
(
    string FirstName,
    string LastName,
    string Email,
    string CountryCode,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    ClientType ClientType = ClientType.Regular,
    TimeOnly? PreferredContactTime = null,
    string? ReferralSource = null
) : ICommand<Result<Guid>>;
