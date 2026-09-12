using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

/// <summary>
/// Whether a PetWalker has published any weekly availability at all.
/// Used by the UI to distinguish "this walker has not set availability"
/// from "the requested day has no free slots".
/// </summary>
public record HasPetWalkerAvailabilityQuery(Guid PetWalkerId) : IRequest<bool>;
