using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.LocationAggregate;
using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options,
  IDomainEventDispatcher? dispatcher) : DbContext(options)
{
  private readonly IDomainEventDispatcher? _dispatcher = dispatcher;

  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<PetWalker> PetWalkers => Set<PetWalker>();
  public DbSet<Photo> Photos => Set<Photo>();
  public DbSet<ServiceArea> ServiceAreas => Set<ServiceArea>();
  public DbSet<Client> Clients => Set<Client>();
  public DbSet<Locality> Localities => Set<Locality>();
  public DbSet<Region> Regions => Set<Region>();
  public DbSet<Country> Countries => Set<Country>();
  public DbSet<Pet> Pets => Set<Pet>();
  public DbSet<Breed> Breeds => Set<Breed>();
  public DbSet<Species> Species => Set<Species>();




  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<HasDomainEventsBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
