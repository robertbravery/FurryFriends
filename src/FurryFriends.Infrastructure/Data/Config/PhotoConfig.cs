using FurryFriends.Core.UserAggregate;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
  public void Configure(EntityTypeBuilder<Photo> builder)
  {
    builder.ToTable("Photos");
    builder.HasKey(p => p.Id);

    builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
    builder.Property(p => p.Url).IsRequired().HasMaxLength(2048);

   //builder.HasOne(p => p.User).WithOne(u => u.BioPicture).HasForeignKey<Photo>("UserId");
  }
}
