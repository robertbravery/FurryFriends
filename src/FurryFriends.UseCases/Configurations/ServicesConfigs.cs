using FurryFriends.UseCases.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FurryFriends.UseCases.Configurations;
public static class ServicesConfigs
{
  public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
  {
    services.AddScoped<IPetWalkerService, PetWalkerService>();
    services.AddScoped<IClientService, ClientService>();
    services.AddScoped<ILogger>(provider => Serilog.Log.Logger);

    return services;
  }
}
