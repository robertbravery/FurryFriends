using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FurryFriends.Infrastructure.Data.Config;

public class PetWalkerConfiguration : IEntityTypeConfiguration<PetWalker>
{
  public void Configure(EntityTypeBuilder<PetWalker> builder)
  {
    builder.ToTable("Users");
    builder.HasKey(u => u.Id);

    builder.Property(u => u.Id).IsRequired().ValueGeneratedOnAdd().HasColumnType("uniqueidentifier"); // Adjust based on your hashing algorithm
    builder.Property(u => u.Biography)
    .HasMaxLength(2000)
    .HasColumnType("nvarchar(2000)");
    builder.Property(u => u.Biography)
      .HasMaxLength(2000)
      .HasColumnType("nvarchar(2000)");

    builder.Property(u => u.DateOfBirth)
      .IsRequired()
      .HasColumnType("datetime2");

    builder.Property(u => u.IsActive)
      .IsRequired();

    builder.Property(u => u.IsVerified)
      .IsRequired();

    builder.Property(u => u.YearsOfExperience)
      .IsRequired();

    builder.Property(u => u.HasInsurance)
      .IsRequired();

    builder.Property(u => u.HasFirstAidCertificatation)
      .IsRequired();

    builder.Property(u => u.DailyPetWalkLimit)
      .IsRequired();

    builder.OwnsOne(p => p.Email, e =>
        {
          e.Property(p => p.EmailAddress)
              .HasColumnName("Email")
              .HasMaxLength(256)
              .IsRequired();

          e.HasIndex(p => p.EmailAddress)
              .IsUnique();
        });

    //builder.HasIndex(u => u.Email).IsUnique();

    builder.OwnsOne(c => c.Name, n =>
    {
      n.Property(p => p.FirstName).HasColumnName("FirstName").HasColumnOrder(1);
      n.Property(p => p.LastName).HasColumnName("LastName").HasColumnOrder(2);
    });

    builder.OwnsOne(c => c.PhoneNumber, n =>
    {
      n.Property(p => p.CountryCode).HasColumnName("PhoneCountryCode").HasMaxLength(3);
      n.Property(p => p.Number).HasColumnName("PhoneNumber").HasMaxLength(15);
    });

    builder.OwnsOne(a => a.Address, a =>
    {
      a.Property(p => p.Street).HasColumnName("Street").HasMaxLength(100);
      a.Property(p => p.City).HasColumnName("City").HasMaxLength(50);
      a.Property(p => p.StateProvinceRegion).HasColumnName("StateProvinceRegion").HasMaxLength(30);
      a.Property(p => p.Country).HasColumnName("Country").HasMaxLength(30);
      a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasMaxLength(5);

    });

    builder.OwnsOne(g => g.Gender, g =>
      g.Property(p => p.Gender)
      .HasColumnName("Gender")
      .HasConversion<int>()
      .IsRequired()
      .HasDefaultValue(GenderType.GenderCategory.NotSelected)
      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw)); // Set sentinel value;

    builder.HasMany(u => u.Photos)
      .WithOne(p => p.User)
      .HasForeignKey(p => p.UserId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.Cascade);

    builder.OwnsOne(u => u.Compensation, c =>
    {
      c.Property(p => p.HourlyRate)
              .HasColumnName("HourlyRate")
              .HasColumnType("decimal(18,2)")
              .IsRequired();

      c.Property(p => p.Currency)
              .HasColumnName("Currency")
              .HasColumnType("char")
              .HasMaxLength(3)
              .IsFixedLength()
              .IsRequired();
    });


    builder.HasMany(sa => sa.ServiceAreas)
        .WithOne(u => u.User)
        .HasForeignKey(sa => sa.UserID).OnDelete(DeleteBehavior.Cascade);
  }
}
