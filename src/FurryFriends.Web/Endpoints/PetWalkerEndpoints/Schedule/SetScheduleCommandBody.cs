namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Schedule;

public record SetScheduleCommandBody(DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
