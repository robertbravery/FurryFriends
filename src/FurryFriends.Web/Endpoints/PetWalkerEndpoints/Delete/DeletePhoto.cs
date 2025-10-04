﻿﻿using Ardalis.SharedKernel;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Delete;

public class DeletePhoto : Endpoint<DeletePhotoRequest, Result>
{
  private readonly IRepository<PetWalker> _repository;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly ILogger<DeletePhoto> _logger;

  public DeletePhoto(
      IRepository<PetWalker> repository,
      IWebHostEnvironment webHostEnvironment,
      ILogger<DeletePhoto> logger)
  {
    _repository = repository;
    _webHostEnvironment = webHostEnvironment;
    _logger = logger;
  }

  public override void Configure()
  {
    Delete(DeletePhotoRequest.Route);
    AllowAnonymous(); // Or apply appropriate authorization
    Options(x => x.WithName("DeletePetWalkerPhoto_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<Result>(200)
        .Produces(400)
        .Produces(404)
        .WithTags("Petwalker Photos"));
  }

  public override async Task HandleAsync(DeletePhotoRequest req, CancellationToken ct)
  {
    try
    {
      // Get the pet walker with photos
      var spec = new GetPetWalkerPicturesSpecification(req.PetWalkerId);
      var petWalker = await _repository.FirstOrDefaultAsync(spec, ct);

      if (petWalker == null)
      {
        _logger.LogWarning("PetWalker with ID {PetWalkerId} not found", req.PetWalkerId);
        await SendNotFoundAsync(ct);
        return;
      }

      // Find the photo to delete
      var photo = petWalker.Photos.FirstOrDefault(p => p.Id == req.PhotoId);
      if (photo == null)
      {
        _logger.LogWarning("Photo with ID {PhotoId} not found for PetWalker {PetWalkerId}",
            req.PhotoId, req.PetWalkerId);
        await SendNotFoundAsync(ct);
        return;
      }

      // Delete the file if it's a local file
      if (photo.Url.StartsWith("/photos/"))
      {
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, photo.Url.TrimStart('/'));
        if (File.Exists(filePath))
        {
          File.Delete(filePath);
        }
      }

      // Remove the photo from the pet walker
      petWalker.Photos.Remove(photo);

      // Save changes to the database
      await _repository.UpdateAsync(petWalker, ct);

      await SendOkAsync(Result.Success(), ct);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting photo {PhotoId} for PetWalker {PetWalkerId}",
          req.PhotoId, req.PetWalkerId);
      await SendErrorsAsync(400, ct);
    }
  }
}
