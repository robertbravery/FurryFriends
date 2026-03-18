using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public record GetWorkingHoursResponse(List<WorkingHoursDto> WorkingHours);
