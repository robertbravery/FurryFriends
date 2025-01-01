using FurryFriends.Core.Common;

namespace FurryFriends.Core.UserAggregate;
public class Photo : AuditableEntity, IEquatable<Photo>
{
  public string Url { get; private set; } = default!;
  public string? Description { get; set; } = default!;

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
