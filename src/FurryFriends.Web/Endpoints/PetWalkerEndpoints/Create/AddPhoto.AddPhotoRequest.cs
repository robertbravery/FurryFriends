﻿namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

public class AddPhotoRequest
{
  public const string Route = "/PetWalker/{PetWalkerId:guid}/photos";

  public Guid PetWalkerId { get; set; }

  // FastEndpoints automatically binds the uploaded file if named correctly
  public IFormFile File { get; set; } = default!;
  public string? Description { get; set; }

  // Optional parameter to specify the photo type (default is PetWalkerPhoto)
  public string? PhotoType { get; set; }
}
