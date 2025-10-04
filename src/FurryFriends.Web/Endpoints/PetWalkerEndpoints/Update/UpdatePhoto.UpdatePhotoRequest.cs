﻿namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Update;

public class UpdatePhotoRequest
{
  public const string Route = "/PetWalker/{PetWalkerId:guid}/photos/{PhotoId:guid}";
  
  public Guid PetWalkerId { get; set; }
  public Guid PhotoId { get; set; }

  // FastEndpoints automatically binds the uploaded file if named correctly
  public IFormFile File { get; set; } = default!;
  public string? Description { get; set; }
}
