using System.Text.Json.Serialization;

namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public record UserListResponseDto
(
    Guid Id,
    [property: JsonPropertyName("Full Name")] string Name = "",
    [property: JsonPropertyName("Email Address")] string EmailAddress = "",
    [property: JsonPropertyName("City Residence")] string City = ""
);
