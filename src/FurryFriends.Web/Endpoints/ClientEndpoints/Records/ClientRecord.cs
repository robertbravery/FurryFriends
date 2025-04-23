using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Records;

public record ClientRecord(Guid Id,
    string Name,
    string Email,
    string PhoneCountryCode,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    ClientType ClientType,
    TimeOnly? PreferredContactTime,
    ReferralSource? ReferralSource,
    List<PetRecord> Pets
    );
