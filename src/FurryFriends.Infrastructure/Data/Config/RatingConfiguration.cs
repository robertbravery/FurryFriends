using FurryFriends.Core.RatingAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurryFriends.Infrastructure.Data.Config;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("Ratings");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(r => r.PetWalkerId)
            .IsRequired();

        builder.Property(r => r.ClientId)
            .IsRequired();

        builder.Property(r => r.BookingId)
            .IsRequired();

        builder.Property(r => r.RatingValue)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .HasColumnType("nvarchar(1000)");

        builder.Property(r => r.CreatedDate)
            .IsRequired();

        builder.Property(r => r.ModifiedDate)
            .IsRequired(false);

        // Unique constraint on BookingId - one rating per booking
        builder.HasIndex(r => r.BookingId)
            .IsUnique();

        // Index for PetWalker ratings query performance
        builder.HasIndex(r => r.PetWalkerId);

        // Index for date-based queries
        builder.HasIndex(r => r.CreatedDate);
    }
}
