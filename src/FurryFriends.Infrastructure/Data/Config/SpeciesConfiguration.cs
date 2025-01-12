using FurryFriends.Core.ClientAggregate;

namespace FurryFriends.Infrastructure.Data.Config;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{

  public void Configure(EntityTypeBuilder<Species> builder)
  {
    // Configure properties and relationships here
    builder.ToTable("Species"); // Specify the table name if different from the entity name
    builder.HasKey(b => b.Id); // Assuming there's an Id property
    builder.Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();

    builder.Property(b => b.Name)
        .IsRequired() // Set to true if Name is required
        .HasMaxLength(100); // Set maximum length if needed

    builder.Property(b => b.Description)
        .HasMaxLength(150); // Set maximum length if needed

  }
}

