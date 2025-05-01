// Example: Endpoints/PetWalkerEndpoints/Photos/UpdateBioPicture.cs
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using FurryFriends.UseCases.Domain.PetWalkers.Query.GetPicture;
using FurryFriends.UseCases.Services.PictureService;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

// Assuming your command returns the updated Photo DTO
public class GetPicture(IMediator mediator) : Endpoint<GetPictureRequest, DetailPictureDto>
{
  public override void Configure()
  {
    Get(GetPictureRequest.Route);
    //AllowFileUploads(); // Important for file uploads
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("UpdatePetWalkerBioPicture_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<PhotoDto>(200)
        .Produces(400)
        .Produces(404)
        .WithTags("Petwalker Photos"));
  }

  public override async Task HandleAsync(GetPictureRequest req, CancellationToken ct)
  {


    var query = new GetPetWalkerPhotoQuery(req.PetWalkerId);

    //TODO: Use the mediator to send the command
    var result = await mediator.Send(query, ct);

    // Handle potential errors from the command handler (e.g., validation, not found)
    // Assuming your handler returns Result<PhotoDto> or similar
    if (!result.IsSuccess)
    {
      await SendResultAsync(Results.BadRequest(result)); // Or NotFound, etc.
      return;
    }

    await SendAsync(result.Value, 200, ct);
  }
}

// --- Similarly, create endpoints for: ---
// POST /api/petwalkers/{PetWalkerId}/photos (using Files.GetFiles("files") for multiple) -> AddPhotosCommand
// DELETE /api/petwalkers/{PetWalkerId}/photos/{PhotoId} -> DeletePhotoCommand
