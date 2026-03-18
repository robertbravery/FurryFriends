using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

public record GetWorkingHoursQuery(Guid PetWalkerId) : IQuery<Result<List<WorkingHoursDto>>>;
