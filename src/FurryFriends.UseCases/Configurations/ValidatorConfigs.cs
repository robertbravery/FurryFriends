using FluentValidation;
using FurryFriends.UseCase.Clients.CreateClient;
using FurryFriends.UseCase.Users.CreatePetWalker;
using FurryFriends.UseCase.Users.UpdateUser;
using Microsoft.Extensions.DependencyInjection;

namespace FurryFriends.UseCase.Configurations;

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
