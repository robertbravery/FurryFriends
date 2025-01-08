using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.Infrastructure.Data.Config;

public class ServiceAreaConfiguration : IEntityTypeConfiguration<ServiceArea>
{
  public void Configure(EntityTypeBuilder<ServiceArea> builder)
  {
    builder.ToTable("ServiceAreas");

    builder.HasKey(sa => sa.Id);

    builder.Property(sa => sa.UserID).IsRequired();
    builder.Property(sa => sa.LocalityID).IsRequired();


    builder.HasOne(sa => sa.Locality)
        .WithMany()
        .HasForeignKey(sa => sa.LocalityID).OnDelete(DeleteBehavior.Restrict);
  }
}
