using FurryFriends.Core.Interfaces;
using FurryFriends.Infrastructure.Data;
using FurryFriends.Infrastructure.Messaging;
using ILogger = Serilog.ILogger;

namespace FurryFriends.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    string environmentName)
  {
    if (environmentName == "Development")
    {
      RegisterDevelopmentOnlyDependencies(services, config);
    }
    else if (environmentName == "Testing")
    {
      RegisterTestingOnlyDependencies(services);
    }
    else
    {
      RegisterProductionOnlyDependencies(services, config);
    }

    RegisterDefaultDependencies(services, logger);
    RegisterEFRepositories(services);

    logger.Information("{Project} services registered", "Infrastructure");

    return services;

  }

  private static void RegisterDefaultDependencies(IServiceCollection services, ILogger logger)
  {
    //services
    //       .AddScoped<IDeleteContributorService, DeleteContributorService>();
  }

  private static void RegisterEFRepositories(IServiceCollection services)
  {
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
  }

  private static void RegisterProductionOnlyDependencies(IServiceCollection services, ConfigurationManager config)
  {
    AddDbContextWithSqlServer(services, config);
    services.AddScoped<IEmailSender, SmtpEmailSender>();
  }

  private static void RegisterTestingOnlyDependencies(IServiceCollection services)
  {
    // do not configure a DbContext here for testing - it's configured in CustomWebApplicationFactory

    services.AddScoped<IEmailSender, FakeEmailSender>();
  }

  private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services, ConfigurationManager config)
  {
    AddDbContextWithSqlServer(services, config);
    services.AddScoped<IEmailSender, SmtpEmailSender>();
  }

  private static void AddDbContextWithSqlServer(IServiceCollection services, ConfigurationManager config)
  {
    var connectionString = config.GetConnectionString("FurryFriendsSqlConnection");
    services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(connectionString));
  }
}
