using FurryFriends.Core.TimeslotAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurryFriends.Infrastructure.Data.Config;

public class TravelBufferConfiguration : IEntityTypeConfiguration<TravelBuffer>
{
    public void Configure(EntityTypeBuilder<TravelBuffer> builder)
    {
        builder.ToTable("TravelBuffers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(t => t.BookingId)
            .IsRequired();

        builder.Property(t => t.PreviousBookingId)
            .IsRequired(false);

        builder.Property(t => t.OriginAddress)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)")
            .IsRequired();

        builder.Property(t => t.DestinationAddress)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)")
            .IsRequired();

        builder.Property(t => t.BufferDurationMinutes)
            .IsRequired();

        builder.Property(t => t.StartTime)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.EndTime)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Index for efficient queries
        builder.HasIndex(t => t.BookingId);
        builder.HasIndex(t => t.PreviousBookingId);
    }
}
