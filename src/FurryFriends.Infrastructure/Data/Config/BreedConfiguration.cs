using FurryFriends.Core.ClientAggregate;

namespace FurryFriends.Infrastructure.Data.Config;
public class BreedConfiguration : IEntityTypeConfiguration<Breed>
{

  public void Configure(EntityTypeBuilder<Breed> builder)
  {
    // Configure properties and relationships here
    builder.ToTable("Breeds"); // Specify the table name if different from the entity name
    builder.HasKey(b => b.Id); // Assuming there's an Id property
    builder.Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();

    builder.Property(b => b.Name)
        .IsRequired() // Set to true if Name is required
        .HasMaxLength(100); // Set maximum length if needed

    builder.Property(b => b.Description)
        .HasMaxLength(150); // Set maximum length if needed

    builder.HasOne(b => b.Species)
        .WithMany(s => s.Breeds)
        .HasForeignKey(b => b.SpeciesId)
        .OnDelete(DeleteBehavior.NoAction);

  }
}

