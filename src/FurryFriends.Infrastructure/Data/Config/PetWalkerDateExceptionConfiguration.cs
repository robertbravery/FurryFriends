using FurryFriends.Core.PetWalkerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurryFriends.Infrastructure.Data.Config;

public class PetWalkerDateExceptionConfiguration : IEntityTypeConfiguration<PetWalkerDateException>
{
  public void Configure(EntityTypeBuilder<PetWalkerDateException> builder)
  {
    builder.ToTable("PetWalkerDateExceptions");

    builder.HasKey(e => e.Id);

    builder.Property(e => e.Id)
        .ValueGeneratedOnAdd()
        .IsRequired();

    builder.Property(e => e.PetWalkerId)
        .IsRequired();

    builder.Property(e => e.Date)
        .IsRequired();

    builder.Property(e => e.IsUnavailable)
        .IsRequired();

    builder.Property(e => e.StartTime)
        .IsRequired(false);

    builder.Property(e => e.EndTime)
        .IsRequired(false);

    builder.Property(e => e.Reason)
        .HasMaxLength(200)
        .HasColumnType("nvarchar(200)")
        .IsRequired(false);

    builder.Property(e => e.CreatedAt)
        .IsRequired()
        .HasColumnType("datetime2");

    builder.Property(e => e.UpdatedAt)
        .IsRequired()
        .HasColumnType("datetime2");

    // A walker can have at most one exception per date; the availability calculator
    // assumes a single override and takes the first match.
    builder.HasIndex(e => new { e.PetWalkerId, e.Date })
        .IsUnique();

    builder.HasOne<PetWalker>()
        .WithMany()
        .HasForeignKey(e => e.PetWalkerId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
