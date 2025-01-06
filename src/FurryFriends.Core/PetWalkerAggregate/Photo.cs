using FurryFriends.Core.Common;
using FurryFriends.Core.UserAggregate.Enums;

namespace FurryFriends.Core.UserAggregate;
public class Photo : AuditableEntity<Guid>, IEquatable<Photo>
{
  public Guid UserId { get; set; }
  public string Url { get; private set; } = default!;
  public PhotoType PhotoType { get; set; }
  public string? Description { get; set; } = default!;

  public virtual PetWalker User { get; set; } = default!;


  // Constructor and methods
  public Photo()
  {
   
  }

  public Photo(string url, string? description = null)
  {
    Description = description;
    Url = Guard.Against.NullOrEmpty(url, nameof(url));
    Guard.Against.InvalidFormat(url, nameof(url),
        @"^https?:\/\/.*\.(png|jpg|jpeg|gif)$",
        "URL must be a valid image URL");
  }

  public bool Equals(Photo? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;

    return Id.Equals(other.Id);
  }

  public override bool Equals(object? obj)
  {
    return obj is Photo photo && Equals(photo);
  }

  public override int GetHashCode()
  {
    return Id.GetHashCode();
  }
}
