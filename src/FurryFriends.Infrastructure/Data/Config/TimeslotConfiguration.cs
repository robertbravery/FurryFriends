using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurryFriends.Infrastructure.Data.Config;

public class TimeslotConfiguration : IEntityTypeConfiguration<Timeslot>
{
    public void Configure(EntityTypeBuilder<Timeslot> builder)
    {
        builder.ToTable("Timeslots");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(t => t.PetWalkerId)
            .IsRequired();

        builder.Property(t => t.Date)
            .IsRequired();

        builder.Property(t => t.StartTime)
            .IsRequired();

        builder.Property(t => t.EndTime)
            .IsRequired();

        builder.Property(t => t.DurationInMinutes)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(TimeslotStatus.Available);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Index for efficient queries
        builder.HasIndex(t => t.PetWalkerId);
        builder.HasIndex(t => t.Date);
        builder.HasIndex(t => new { t.PetWalkerId, t.Date });
        builder.HasIndex(t => t.Status);
    }
}
