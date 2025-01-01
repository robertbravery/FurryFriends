using FurryFriends.Core.ContributorAggregate;

namespace FurryFriends.Infrastructure.Data.Config;

public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
{
  public void Configure(EntityTypeBuilder<Contributor> builder)
  {
    //builder.Property(p => p.Name)
    //    .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
    //    .IsRequired();

    builder.OwnsOne(c => c.Name, n =>
    {
      n.Property(p => p.FirstName).HasColumnName("FirstName");
      n.Property(p => p.LastName).HasColumnName("LastName");
    });

    builder.OwnsOne(builder => builder.PhoneNumber);

   

    builder.Property(x => x.Status)
      .HasConversion(
          x => x.Value,
          x => ContributorStatus.FromValue(x));
  }
}
