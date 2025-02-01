using System.Reflection;
using Ardalis.SharedKernel;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.PetWalkers.CreatePetWalker;

namespace FurryFriends.Web.Configurations;

public static class MediatrConfigs
{
  public static IServiceCollection AddMediatrConfigs(this IServiceCollection services)
  {
    var mediatRAssemblies = new[]
      {
        Assembly.GetAssembly(typeof(PetWalker)), // Core
        Assembly.GetAssembly(typeof(CreatePetWalkerCommand)) // UseCases
      };

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

    return services;
  }
}
