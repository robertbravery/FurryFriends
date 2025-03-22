using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.Infrastructure.Data.Config;
internal class PetConfiguration : IEntityTypeConfiguration<Pet>
{
  public void Configure(EntityTypeBuilder<Pet> builder)
  {
    builder.ToTable("Pets");

    builder.HasKey(p => p.Id);
    builder.Property(p => p.Id).ValueGeneratedOnAdd();


    builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
    builder.Property(p => p.Color).IsRequired().HasMaxLength(100);
    builder.Property(p => p.Gender).HasConversion<int>().HasDefaultValue(PetGender.Unknown);
    builder.Property(p => p.BreedId);
    builder.Property(p => p.MedicalConditions).HasMaxLength(500);
    builder.Property(p => p.DietaryRestrictions).HasMaxLength(500);
    builder.Property(p => p.FavoriteActivities).HasMaxLength(500);
    builder.Property(p => p.IsActive).IsRequired();
    builder.Property(p => p.IsVaccinated).IsRequired();
    builder.Property(p => p.IsSterilized).IsRequired();

    builder.HasOne(p => p.Owner)
          .WithMany(c => c.Pets)
          .HasForeignKey(p => p.OwnerId)
          .IsRequired();

    builder.HasOne(p => p.BreedType)
        .WithMany(p => p.Pets)
        .HasForeignKey(p => p.BreedId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
