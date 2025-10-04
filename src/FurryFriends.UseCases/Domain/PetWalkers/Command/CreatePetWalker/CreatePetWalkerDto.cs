using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;


public record CreatePetWalkerDto(
    Name Name,
    Email Email,
    PhoneNumber PhoneNumber,
    Address Address,
    GenderType Gender,
    string Biography,
    DateTime DateOfBirth,
    bool IsActive,
    bool IsVerified,
    int YearsOfExperience,
    bool HasInsurance,
    bool HasFirstAidCertification,
    int DailyPetWalkLimit,
    Compensation Compensation);

