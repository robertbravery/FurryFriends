//using FastEndpoints;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using Mediator;
//using MediatR;

public record SetScheduleCommand(Guid PetWalkerId, List<ScheduleDto> Schedules)
    : ICommand<Mediator.Unit>;
