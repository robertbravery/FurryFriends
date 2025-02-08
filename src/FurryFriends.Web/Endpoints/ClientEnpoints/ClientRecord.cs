using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.Web.Endpoints.ClientEnpoints;

public record ClientRecord(Guid Id,
    string Name,
    string Email,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string ZipCode,
    ClientType ClientType,
    TimeOnly? PreferredContactTime,
    string? ReferralSource);
