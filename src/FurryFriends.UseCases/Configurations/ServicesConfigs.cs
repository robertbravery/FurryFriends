using FurryFriends.UseCases.PipeLineBehaviours;
using FurryFriends.UseCases.Services.ClientService;
using FurryFriends.UseCases.Services.LocationService;
using FurryFriends.UseCases.Services.PetWalkerService;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FurryFriends.UseCases.Configurations;
public static class ServicesConfigs
{
  public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
  {
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    services.AddScoped<IPetWalkerService, PetWalkerService>();
    services.AddScoped<IClientService, ClientService>();
    services.AddScoped<ILocationService, LocationService>();
    services.AddScoped<IServiceAreaService, ServiceAreaService>();
    services.AddScoped(provider => Log.Logger);

    return services;
  }
}
