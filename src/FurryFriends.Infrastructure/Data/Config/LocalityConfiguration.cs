using FurryFriends.Core.LocationAggregate;

public class LocalityConfiguration : IEntityTypeConfiguration<Locality>
{
  public void Configure(EntityTypeBuilder<Locality> builder)
  {
    builder.ToTable("Localities");

    builder.HasKey(l => l.Id);

    builder.Property(l => l.LocalityName)
        .IsRequired()
        .HasMaxLength(100);

    builder.HasOne(l => l.Region)
        .WithMany(r => r.Localities)
        .HasForeignKey(l => l.RegionID).OnDelete(DeleteBehavior.NoAction);
  }
}
