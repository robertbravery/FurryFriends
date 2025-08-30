using FurryFriends.Core.PetWalkerAggregate.Enums;
using FurryFriends.UseCases.Services.PictureService;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

public class AddPhoto(IMediator mediator) : Endpoint<AddPhotoRequest, DetailedPhotoDto>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AddPhotoRequest.Route);
    AllowFileUploads(); // Important for file uploads
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("AddPetWalkerPhoto_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<DetailedPhotoDto>(201)
        .Produces(400)
        .Produces(404)
        .WithTags("Petwalker Photos"));
  }

  public override async Task HandleAsync(AddPhotoRequest req, CancellationToken ct)
  {
    // Get the picture service directly since we don't have a command/handler for this yet
    var pictureService = HttpContext.RequestServices.GetRequiredService<IPictureService>();

    // Determine the photo type
    var photoType = PhotoType.PetWalkerPhoto; // Default
    if (!string.IsNullOrEmpty(req.PhotoType) && Enum.TryParse<PhotoType>(req.PhotoType, out var parsedType))
    {
      photoType = parsedType;
    }

    var result = await pictureService.AddPetWalkerPhotoAsync(
        req.PetWalkerId,
        req.File,
        req.Description,
        photoType,
        ct);

    // Handle potential errors
    if (!result.IsSuccess)
    {
      if (result.Status == ResultStatus.NotFound)
      {
        await SendNotFoundAsync(ct);
        return;
      }

      await SendResultAsync(Results.BadRequest(result));
      return;
    }

    var photo = result.Value;
    var response = new DetailedPhotoDto
    {
      Id = photo.Id,
      PhotoType = photo.PhotoType,
      Url = photo.Url,
      Description = photo.Description
    };

    await SendCreatedAtAsync<GetPicture>(
        new { PetWalkerId = req.PetWalkerId },
        response,
        generateAbsoluteUrl: true,
        cancellation: ct);
  }
}
