using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using MediatR;

public record SetScheduleCommand(Guid PetWalkerId, List<ScheduleDto> Schedules)
    : ICommand<Unit>;
