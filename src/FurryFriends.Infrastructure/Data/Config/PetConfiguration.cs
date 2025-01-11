using FurryFriends.Core.ClientAggregate;

namespace FurryFriends.Infrastructure.Data.Config;
internal class PetConfiguration : IEntityTypeConfiguration<Pet>
{
  public void Configure(EntityTypeBuilder<Pet> builder)
  {
    builder.ToTable("Pets");

    builder.HasKey(p => p.Id);

    builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
    builder.Property(p => p.BreedId);
    builder.Property(p => p.Species).HasMaxLength(50);

    builder.HasOne(p => p.Owner)
        .WithMany(c => c.Pets)
        .HasForeignKey(p => p.OwnerId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.
        HasOne(p => p.BreedType)
        .WithMany()
        .HasForeignKey(p => p.BreedId)
        .OnDelete(DeleteBehavior.Restrict);

  }
}
