using FurryFriends.Core.Common;
using FurryFriends.Core.PetWalkerAggregate.Enums;

namespace FurryFriends.Core.PetWalkerAggregate;
public class Photo : AuditableEntity<Guid>, IEquatable<Photo>
{
  public Guid UserId { get; set; }
  public string Url { get; set; } = default!;
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
    SetUrl(url);
  }

  public void SetUrl(string url)
  {
    Url = Guard.Against.NullOrEmpty(url, nameof(url));

    // Allow both HTTP URLs and local file paths
    if (url.StartsWith("http"))
    {
      Guard.Against.InvalidFormat(url, nameof(url),
          @"^https?:\/\/.*\.(png|jpg|jpeg|gif)$",
          "URL must be a valid image URL");
    }
    else if (url.StartsWith("/photos/"))
    {
      // Local file path is allowed
    }
    else
    {
      throw new ArgumentException("URL must be a valid image URL or a local file path", nameof(url));
    }
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
