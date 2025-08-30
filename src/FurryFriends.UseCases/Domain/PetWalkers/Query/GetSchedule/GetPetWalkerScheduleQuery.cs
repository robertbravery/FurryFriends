using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

/// <summary>
/// Query to get the basic schedule for a PetWalker
/// </summary>
public record GetPetWalkerScheduleQuery(Guid PetWalkerId) : IRequest<Result<List<ScheduleDto>>>;
