using Microsoft.Extensions.DependencyInjection;
using FurryFriends.UseCases.Services;
using System.Net.Security;
using Serilog;

namespace FurryFriends.UseCases.Configurations;
public static class ServicesConfigs
{
  public static IServiceCollection AddUseCaseServices(this IServiceCollection services)
  {
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<ILogger>(provider => Serilog.Log.Logger);

    return services;
  }
}
