using FurryFriends.Core.LocationAggregate;

namespace FurryFriends.Infrastructure.Data.Config;
public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
  public void Configure(EntityTypeBuilder<Country> builder)
  {
    builder.ToTable("Countries");

    builder.HasKey(c => c.Id);

    builder.Property(c => c.CountryName)
        .IsRequired()
        .HasMaxLength(100);

    builder.HasMany(c => c.Regions)
        .WithOne(r => r.Country)
        .HasForeignKey(r => r.CountryID)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
