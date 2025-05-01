// Example: Endpoints/PetWalkerEndpoints/Photos/UpdateBioPictureRequest.cs
namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

public class GetPictureRequest
{
  public const string Route = "/PetWalker/{PetWalkerId:guid}/photos";
  public Guid PetWalkerId { get; set; }

  // FastEndpoints automatically binds the uploaded file if named correctly
  // or you can explicitly define it:
  // public IFormFile File { get; set; }
}

// --- Similarly, create endpoints for: ---
// POST /api/petwalkers/{PetWalkerId}/photos (using Files.GetFiles("files") for multiple) -> AddPhotosCommand
// DELETE /api/petwalkers/{PetWalkerId}/photos/{PhotoId} -> DeletePhotoCommand
