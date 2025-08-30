using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

public record GetAvailabilityQuery(Guid PetWalkerId, DateTime Date)
    : IRequest<List<AvailableSlotDto>>;
