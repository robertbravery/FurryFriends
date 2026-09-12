using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetAvailability;

public record GetPetWalkerDateExceptionsQuery(Guid PetWalkerId) : IQuery<Result<List<PetWalkerDateExceptionDto>>>;

public record GetPetWalkerAvailabilitySettingsQuery(Guid PetWalkerId)
    : IQuery<Result<PetWalkerAvailabilitySettingsDto>>;
