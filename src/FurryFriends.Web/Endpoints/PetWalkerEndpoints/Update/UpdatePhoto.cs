using FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePhoto;
using FurryFriends.UseCases.Services.PictureService;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Update;

public class UpdatePhoto(IMediator mediator) : Endpoint<UpdatePhotoRequest, DetailedPhotoDto>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(UpdatePhotoRequest.Route);
    AllowFileUploads(); // Important for file uploads
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("UpdatePetWalkerPhoto_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<DetailedPhotoDto>(200)
        .Produces(400)
        .Produces(404)
        .WithTags("Petwalker Photos"));
  }

  public override async Task HandleAsync(UpdatePhotoRequest req, CancellationToken ct)
  {
    var command = new UpdatePhotoCommand
    {
      PetWalkerId = req.PetWalkerId,
      PhotoId = req.PhotoId,
      File = req.File,
      Description = req.Description
    };

    var result = await _mediator.Send(command, ct);

    // Handle potential errors from the command handler
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

    await SendAsync(result.Value, 200, ct);
  }
}
