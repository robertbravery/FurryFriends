﻿namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Delete;

public class DeletePhotoRequest
{
  public const string Route = "/PetWalker/{PetWalkerId:guid}/photos/{PhotoId:guid}";
  
  public Guid PetWalkerId { get; set; }
  public Guid PhotoId { get; set; }
}
