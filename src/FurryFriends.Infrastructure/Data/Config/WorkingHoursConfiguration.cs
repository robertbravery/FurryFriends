using FurryFriends.Core.TimeslotAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurryFriends.Infrastructure.Data.Config;

public class WorkingHoursConfiguration : IEntityTypeConfiguration<WorkingHours>
{
    public void Configure(EntityTypeBuilder<WorkingHours> builder)
    {
        builder.ToTable("WorkingHours");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(w => w.PetWalkerId)
            .IsRequired();

        builder.Property(w => w.DayOfWeek)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(w => w.StartTime)
            .IsRequired();

        builder.Property(w => w.EndTime)
            .IsRequired();

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(w => w.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(w => w.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Index for efficient queries
        builder.HasIndex(w => w.PetWalkerId);
        builder.HasIndex(w => w.DayOfWeek);
        builder.HasIndex(w => new { w.PetWalkerId, w.DayOfWeek });
    }
}
