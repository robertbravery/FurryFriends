using FluentValidation;
using FurryFriends.UseCases.Domain.Clients.Command.CreateClient;
using FurryFriends.UseCases.Domain.Clients.Command.RemovePet;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;
using Microsoft.Extensions.DependencyInjection;

namespace FurryFriends.UseCases.Configurations;

public static class ValidatorConfigs
{
  public static IServiceCollection AddUseCaseValidators(this IServiceCollection services)
  {
    services.AddValidatorsFromAssemblyContaining<CreatePetWalkerCommandValidator>();

    services.AddTransient<IValidator<UpdatePetWalkerHourlyRateCommand>, UpdatePetWalkerHourlyRateCommandValidator>();
    services.AddTransient<IValidator<CreateClientCommand>, CreateClientCommandValidator>();
    services.AddTransient<IValidator<CreatePetWalkerCommand>, CreatePetWalkerCommandValidator>();
    services.AddTransient<IValidator<RemovePetCommand>, RemovePetCommandValidator>();

    return services;
  }
}
