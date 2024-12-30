namespace FurryFriends.Web.Endpoints.UserEndpoints.Get;

public record GetUserByEmailResponse(
    Guid Id,
    string FullName = default!,
    string Email = default!,
    string PhoneNumber = default!,
    string City = default!
);
