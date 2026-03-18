using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

public record GetAvailableTimeslotsQuery(
    Guid PetWalkerId,
    DateOnly Date
) : IQuery<Result<GetAvailableTimeslotsResponse>>;

public record GetAvailableTimeslotsResponse(
    Guid PetWalkerId,
    DateOnly Date,
    List<AvailableTimeslotDto> Timeslots,
    bool HasTravelBufferWarning,
    string? TravelBufferMessage);
