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

        builder.Property(r => r.RatingValue)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .HasColumnType("nvarchar(1000)");

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.HasIndex(r => r.PetWalkerId);

        builder.HasIndex(r => r.ClientId);

        builder.HasIndex(r => new { r.PetWalkerId, r.Status });
    }
}