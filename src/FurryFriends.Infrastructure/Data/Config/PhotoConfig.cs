using FurryFriends.Core.PetWalkerAggregate;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
  public void Configure(EntityTypeBuilder<Photo> builder)
  {
    builder.ToTable("Photos");
    builder.HasKey(p => p.Id);

    builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
    builder.Property(p => p.Url).IsRequired().HasMaxLength(2048);
  }
}
