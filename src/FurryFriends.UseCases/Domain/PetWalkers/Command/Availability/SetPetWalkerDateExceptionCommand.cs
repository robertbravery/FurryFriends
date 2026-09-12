using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

/// <summary>
/// Creates or replaces the date-specific availability override for a walker.
/// Exactly one exception may exist per walker per date, so this is an upsert.
/// </summary>
public record SetPetWalkerDateExceptionCommand(
    Guid PetWalkerId,
    DateTime Date,
    bool IsUnavailable,
    TimeOnly? StartTime = null,
    TimeOnly? EndTime = null,
    string? Reason = null) : ICommand<Result<PetWalkerDateExceptionDto>>;
