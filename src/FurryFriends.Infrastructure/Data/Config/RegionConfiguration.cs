using FurryFriends.Core.LocationAggregate;

namespace FurryFriends.Infrastructure.Data.Config;


public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
  public void Configure(EntityTypeBuilder<Region> builder)
  {
    builder.ToTable("Regions");

    builder.HasKey(r => r.Id);

    builder.Property(r => r.RegionName)
        .IsRequired()
        .HasMaxLength(100);

    builder.HasOne(r => r.Country)
        .WithMany(c => c.Regions)
        .HasForeignKey(r => r.CountryID);

    builder.HasMany(r => r.Localities)
        .WithOne(l => l.Region)
        .HasForeignKey(l => l.RegionID)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
