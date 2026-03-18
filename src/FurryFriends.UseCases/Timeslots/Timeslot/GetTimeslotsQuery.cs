using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

public record GetTimeslotsQuery(
    Guid PetWalkerId,
    DateOnly? Date
) : IQuery<Result<GetTimeslotsResponse>>;

public record GetTimeslotsResponse(
    Guid PetWalkerId,
    DateOnly? Date,
    List<TimeslotDto> Timeslots
);
