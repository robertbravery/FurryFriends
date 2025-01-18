using FluentValidation;
using FurryFriends.UseCases.Clients.CreateClient;
using FurryFriends.UseCases.Users.CreatePetWalker;
using FurryFriends.UseCases.Users.UpdateUser;
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
