using FurryFriends.Core.UserAggregate;

namespace FurryFriends.Infrastructure.Data.Config;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");
    builder.HasKey(u => u.Id);

    builder.Property(u => u.Id).IsRequired().ValueGeneratedOnAdd(); // Adjust based on your hashing algorithm
    builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
    builder.HasIndex(u => u.Email).IsUnique();

    builder.OwnsOne(c => c.Name, n =>
    {
      n.Property(p => p.FirstName).HasColumnName("FirstName");
      n.Property(p => p.LastName).HasColumnName("LastName");
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
      a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasMaxLength(5);

    });

  }
}
