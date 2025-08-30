namespace FurryFriends.Infrastructure.Data.Config;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
  public void Configure(EntityTypeBuilder<Booking> builder)
  {
    builder.ToTable("Bookings");

    builder.HasKey(b => b.Id);

    builder.Property(b => b.Id)
        .ValueGeneratedOnAdd()
        .IsRequired();

    builder.Property(b => b.PetWalkerId)
        .IsRequired();

    builder.Property(b => b.PetOwnerId)
        .IsRequired();

    builder.Property(b => b.Start)
        .IsRequired();

    builder.Property(b => b.End)
        .IsRequired();

    builder.HasOne(b => b.PetWalker)
        .WithMany()
        .HasForeignKey(b => b.PetWalkerId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(b => b.PetOwner)
        .WithMany()
        .HasForeignKey(b => b.PetOwnerId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
