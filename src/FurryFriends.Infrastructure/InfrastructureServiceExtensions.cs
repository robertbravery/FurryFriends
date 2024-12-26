using FurryFriends.Core.Interfaces;
using FurryFriends.Core.Services;
using FurryFriends.Infrastructure.Data;
using FurryFriends.Infrastructure.Data.Queries;
using FurryFriends.UseCases.Contributors.List;


namespace FurryFriends.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    //string? connectionString = config.GetConnectionString("SqliteConnection");
    string? connectionString = config.GetConnectionString("FurryFriendsSqlConnection");
    Guard.Against.Null(connectionString);
    services.AddDbContext<AppDbContext>(options =>
     //options.UseSqlite(connectionString));
     options.UseSqlServer(connectionString)); 

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
           .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
           .AddScoped<IDeleteContributorService, DeleteContributorService>();


    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
