namespace FurryFriends.UseCases.Domain.PetWalkers.Dto;

public record ScheduleDto(DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
