using System.Text.Json.Serialization;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public record PetWalkerListResponseDto
(
    //Guid Id,
    //[property: JsonPropertyName("Full Name")] string Name = "",
    //[property: JsonPropertyName("Email Address")] string EmailAddress = "",
    //[property: JsonPropertyName("City Residence")] string City = ""
    Guid Id, string Name, string EmailAddress, string City, string Location
);
