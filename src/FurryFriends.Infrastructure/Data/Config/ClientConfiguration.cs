using FurryFriends.Core.ClientAggregate;

namespace FurryFriends.Infrastructure.Data.Config;
public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
  public void Configure(EntityTypeBuilder<Client> builder)
  {
    builder.ToTable("Clients");

    builder.HasKey(c => c.Id);
    builder.Property(c => c.Id).HasColumnOrder(0).IsRequired().ValueGeneratedOnAdd();

    // Configure Value objects
    builder.OwnsOne(c => c.Name, n =>
    {
      n.Property(p => p.FirstName).HasColumnName("FirstName").HasColumnOrder(1).IsRequired().HasMaxLength(50);
      n.Property(p => p.LastName).HasColumnName("LastName").HasColumnOrder(2).IsRequired().HasMaxLength(50);
    });

    builder.OwnsOne(c => c.Email, e =>
    {
      e.Property(p => p.EmailAddress).HasColumnName("Email").HasColumnOrder(4).IsRequired().HasMaxLength(256);
    });

    builder.OwnsOne(c => c.PhoneNumber, p =>
    {
      p.Property(p => p.CountryCode).HasColumnName("PhoneCountryCode").HasColumnOrder(5).IsRequired().HasMaxLength(3);
      p.Property(p => p.Number).HasColumnName("PhoneNumber").HasColumnOrder(6).IsRequired().HasMaxLength(20);
    });

    builder.OwnsOne(c => c.Address, a =>
    {
      a.Property(p => p.Street).HasColumnName("Street").HasColumnOrder(7).HasMaxLength(100);
      a.Property(p => p.City).HasColumnName("City").HasColumnOrder(8).HasMaxLength(50);
      a.Property(p => p.StateProvinceRegion).HasColumnName("StateProvinceRegion").HasColumnOrder(9).HasMaxLength(30);
      a.Property(p => p.Country).HasColumnName("Country").HasColumnOrder(10).HasMaxLength(30);
      a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasColumnOrder(11).HasMaxLength(5);
    });

    builder.Property(p => p.ClientType).HasColumnOrder(3).HasConversion<int>().IsRequired();
    builder.Property(p => p.ReferralSource).IsRequired(true).HasConversion<int>();
    builder.Property(p => p.UpdatedAt).HasColumnName("UpdatedAt").HasColumnOrder(13);
    builder.Property(p => p.CreatedAt).HasColumnName("CreatedAt").HasColumnOrder(14);
    builder.Property(p => p.PreferredContactTime).HasColumnOrder(15).HasColumnType("time").IsRequired(false);

    builder.HasMany(c => c.Pets)
      .WithOne(p => p.Owner)
      .HasForeignKey(p => p.OwnerId)
      .OnDelete(DeleteBehavior.Cascade);

  }
}
