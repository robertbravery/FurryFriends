using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurryFriends.Infrastructure.Data.Config;

public class CustomTimeRequestConfiguration : IEntityTypeConfiguration<CustomTimeRequest>
{
    public void Configure(EntityTypeBuilder<CustomTimeRequest> builder)
    {
        builder.ToTable("CustomTimeRequests");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(c => c.ClientId)
            .IsRequired();

        builder.Property(c => c.PetWalkerId)
            .IsRequired();

        builder.Property(c => c.RequestedDate)
            .IsRequired();

        builder.Property(c => c.PreferredStartTime)
            .IsRequired();

        builder.Property(c => c.PreferredEndTime)
            .IsRequired();

        builder.Property(c => c.PreferredDurationMinutes)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(CustomTimeRequestStatus.Pending);

        builder.Property(c => c.ClientAddress)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)")
            .IsRequired();

        builder.Property(c => c.CounterOfferedDate)
            .IsRequired(false);

        builder.Property(c => c.CounterOfferedTime)
            .IsRequired(false);

        builder.Property(c => c.ResponseReason)
            .HasMaxLength(1000)
            .HasColumnType("nvarchar(1000)")
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Index for efficient queries
        builder.HasIndex(c => c.ClientId);
        builder.HasIndex(c => c.PetWalkerId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.RequestedDate);
    }
}
