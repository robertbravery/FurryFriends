using FurryFriends.Core.BookingAggregate;

namespace FurryFriends.Infrastructure.Data.Config;

public class CancellationConfiguration : IEntityTypeConfiguration<Cancellation>
{
  public void Configure(EntityTypeBuilder<Cancellation> builder)
  {
    builder.ToTable("Cancellations");

    builder.HasKey(c => c.Id);

    builder.Property(c => c.Id)
        .ValueGeneratedOnAdd()
        .IsRequired();

    builder.Property(c => c.BookingId)
        .IsRequired();

    builder.Property(c => c.CancellationDate)
        .IsRequired();

    builder.Property(c => c.Reason)
        .HasConversion<int>()
        .IsRequired();

    builder.Property(c => c.CancelledBy)
        .HasConversion<int>()
        .IsRequired();

    builder.Property(c => c.AdditionalNotes);

    builder.HasOne(c => c.Booking)
        .WithOne(b => b.Cancellation)
        .HasForeignKey<Cancellation>(c => c.BookingId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}