using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Records;

public record PetWalkerRecord(
    Guid Id,
    string FullName = default!,
    string Email = default!,
    string CountryCode = default!,
    string PhoneNumber = default!,
    string Street = default!,
    string City = default!,
    string State = default!,
    string ZipCode = default!,
    string Country = default!,
    string? Biography = default!,
    DateTime DateOfBirth = default!,
    string Gender = default!,
    decimal HourlyRate = default!,
    string Currency = default!,
    bool IsActive = default!,
    bool IsVerified = default!,
    int YearsOfExperience = default!,
    bool HasInsurance = default!,
    bool HasFirstAidCertification = default!,
    int DailyPetWalkLimit = default!,
    List<string>? Locations = default!,
    PhotoDto? BioPicture = default!,
    List<PhotoDto>? Photos = default!);
