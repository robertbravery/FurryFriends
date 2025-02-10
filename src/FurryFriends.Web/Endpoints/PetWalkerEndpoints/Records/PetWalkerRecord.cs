using FurryFriends.UseCases.Domain.PetWalkers.Query.GetPetWalker;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Records;

public record PetWalkerRecord(Guid Id,
    string FullName = default!,
    string Email = default!,
    string PhoneNumber = default!,
    string City = default!,
    List<string>? Locations = default!,
    PhotoDto? BioPicture = default!,
    List<PhotoDto>? Photos = default!);
