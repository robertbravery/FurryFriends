using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Users.CreateUser;

public record CreatePetWalkerCommand
(
    string FirstName,
    string LastName,
    string Email,
    string CountryCode,
    string Number,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    GenderType.GenderCategory Gender,
    string? Biography,
    DateTime DateOfBirth,
    decimal HourlyRate,
    string Currency,
    bool IsActive,
    bool IsVerified,
    int YearsOfExperience,
    bool HasInsurance,
    bool HasFirstAidCertification,
    int DailyPetWalkLimit
) : ICommand<Result<Guid>>;
