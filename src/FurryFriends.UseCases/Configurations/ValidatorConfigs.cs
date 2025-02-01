using FluentValidation;
using FurryFriends.UseCases.Clients.CreateClient;
using FurryFriends.UseCases.PetWalkers.CreatePetWalker;
using FurryFriends.UseCases.PetWalkers.UpdatePetWalker;
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


    return services;
  }
}
