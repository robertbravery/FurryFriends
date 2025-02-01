using FurryFriends.Infrastructure.Data;
using FurryFriends.UseCases.Clients.CreateClient;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.FunctionalTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
  protected override IHost CreateHost(IHostBuilder builder)
  {
    // Force the environment to Testing so that real external calls (email, etc.) are skipped.
    builder.UseEnvironment("Testing");

    var host = builder.Build();
    host.Start();

    SeedDatabase(host);

    return host;
  }

  /// <summary>
  /// Seeds the in-memory database after ensuring it is deleted and re-created.
  /// </summary>
  /// <param name="host">The built IHost instance.</param>
  private void SeedDatabase(IHost host)
  {
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var logger = services.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

    try
    {
      db.Database.EnsureDeleted();
      db.Database.EnsureCreated();
      // PopulateTestDataAsync is blocking the thread here for simplicity,
      // but consider making this async if appropriate.
      SeedData.PopulateTestDataAsync(db).Wait();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred seeding the database. Error: {exceptionMessage}", ex.Message);
    }
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      // Remove existing DbContext registrations.
      var descriptors = services.Where(
          d => d.ServiceType == typeof(AppDbContext) ||
               d.ServiceType == typeof(DbContextOptions<AppDbContext>))
          .ToList();
      foreach (var descriptor in descriptors)
      {
        services.Remove(descriptor);
      }

      // Each test run gets a unique in-memory database name.
      string inMemoryCollectionName = Guid.NewGuid().ToString();

      // Re-register DbContext for testing using the in-memory database.
      services.AddDbContext<AppDbContext>(options =>
      {
        options.UseInMemoryDatabase(inMemoryCollectionName)
               .LogTo(Console.WriteLine);
      });

      // Register MediatR with the relevant assemblies.
      services.AddMediatR(cfg =>
      {
        cfg.RegisterServicesFromAssemblies(
            typeof(Program).Assembly,
            typeof(CreateClientCommand).Assembly,
            typeof(AppDbContext).Assembly);
      });
    });
  }
}
