﻿using FurryFriends.Core.UserAggregate;

namespace FurryFriends.Infrastructure.Data.Config;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");
    builder.HasKey(u => u.Id);

    builder.Property(u => u.Id).IsRequired().ValueGeneratedOnAdd().HasColumnType("uniqueidentifier"); // Adjust based on your hashing algorithm
    builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
    builder.HasIndex(u => u.Email).IsUnique();

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

    builder.HasMany(u => u.Photos)
      .WithOne(p => p.User)
      .HasForeignKey(p => p.UserId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
