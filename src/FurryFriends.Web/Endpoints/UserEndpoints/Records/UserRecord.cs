using System;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Records;

public record UserRecord(Guid Id,
    string FullName = default!,
    string Email = default!,
    string PhoneNumber = default!,
    string City = default!);
