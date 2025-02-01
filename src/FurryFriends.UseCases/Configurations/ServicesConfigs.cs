using FurryFriends.UseCase.Services;
using FurryFriends.UseCases.PipeLineBehaviours;
using FurryFriends.UseCases.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FurryFriends.UseCase.Configurations;
public static class ServicesConfigs
{
  public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
  {
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    services.AddScoped<IPetWalkerService, PetWalkerService>();
    services.AddScoped<IClientService, ClientService>();
    services.AddScoped(provider => Log.Logger);

    return services;
  }
}
