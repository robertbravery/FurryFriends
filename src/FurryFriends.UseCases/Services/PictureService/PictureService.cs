using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Enums;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Services.PictureService;

public class PictureService : IPictureService
{
  private readonly IRepository<PetWalker> _repository;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly ILogger<PictureService> _logger;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private const string PHOTOS_FOLDER = "photos";

  public PictureService(
      IRepository<PetWalker> repository,
      IWebHostEnvironment webHostEnvironment,
      ILogger<PictureService> logger,
      IHttpContextAccessor httpContextAccessor)
  {
    _repository = repository;
    _webHostEnvironment = webHostEnvironment;
    _logger = logger;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<Result<Photo>> UpdatePetWalkerBioPictureAsync(Guid petWalkerId, IFormFile file, CancellationToken cancellationToken)
  {
    try
    {
      // Get the pet walker
      var spec = new GetPetWalkerPicturesSpecification(petWalkerId);
      var petWalker = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      if (petWalker == null)
      {
        return Result.NotFound($"PetWalker with ID {petWalkerId} not found");
      }

      // Find existing bio picture or create a new one
      var bioPicture = petWalker.Photos.FirstOrDefault(p => p.PhotoType == PhotoType.BioPic);

      // If bio picture exists, update it
      if (bioPicture != null)
      {
        // Delete the old file if it's a local file
        if (bioPicture.Url.StartsWith("/photos/"))
        {
          var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, bioPicture.Url.TrimStart('/'));
          if (File.Exists(oldFilePath))
          {
            File.Delete(oldFilePath);
          }
        }

        // Save the new file
        var fileUrl = await SaveFileAsync(file, petWalkerId, PhotoType.BioPic);

        // Update the photo entity
        bioPicture.Url = fileUrl;
        bioPicture.Description = "Profile Picture";
      }
      else
      {
        // Create a new bio picture
        var fileUrl = await SaveFileAsync(file, petWalkerId, PhotoType.BioPic);
        bioPicture = new Photo(fileUrl, "Profile Picture")
        {
          UserId = petWalkerId,
          PhotoType = PhotoType.BioPic
        };
        petWalker.Photos.Add(bioPicture);
      }

      // Save changes to the database
      await _repository.UpdateAsync(petWalker, cancellationToken);

      return Result.Success(bioPicture);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating bio picture for PetWalker {PetWalkerId}", petWalkerId);
      return Result.Error($"Error updating bio picture: {ex.Message}");
    }
  }

  public async Task<Result<PetWalker>> GetPetWalkerPictureAsync(Guid petWalkerId, CancellationToken cancellationToken)
  {
    var spec = new GetPetWalkerPicturesSpecification(petWalkerId, true);
    var petwalker = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (petwalker is null)
    {
      return Result.NotFound();
    }
    return petwalker;
  }

  public async Task<Result<Photo>> UpdatePetWalkerPhotoAsync(
      Guid petWalkerId,
      Guid photoId,
      IFormFile file,
      string? description,
      CancellationToken cancellationToken)
  {
    try
    {
      // Get the pet walker
      var spec = new GetPetWalkerPicturesSpecification(petWalkerId);
      var petWalker = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      if (petWalker == null)
      {
        return Result.NotFound($"PetWalker with ID {petWalkerId} not found");
      }

      // Find the photo to update
      var photo = petWalker.Photos.FirstOrDefault(p => p.Id == photoId);
      if (photo == null)
      {
        return Result.NotFound($"Photo with ID {photoId} not found");
      }

      // Delete the old file if it's a local file
      if (photo.Url.StartsWith("/photos/"))
      {
        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, photo.Url.TrimStart('/'));
        if (File.Exists(oldFilePath))
        {
          File.Delete(oldFilePath);
        }
      }

      // Save the new file
      var fileUrl = await SaveFileAsync(file, petWalkerId, photo.PhotoType);

      // Update the photo entity
      photo.Url = fileUrl;
      if (!string.IsNullOrWhiteSpace(description))
      {
        photo.Description = description;
      }

      // Save changes to the database
      await _repository.UpdateAsync(petWalker, cancellationToken);

      return Result.Success(photo);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating photo {PhotoId} for PetWalker {PetWalkerId}", photoId, petWalkerId);
      return Result.Error($"Error updating photo: {ex.Message}");
    }
  }

  public async Task<Result<Photo>> AddPetWalkerPhotoAsync(
      Guid petWalkerId,
      IFormFile file,
      string? description,
      PhotoType photoType,
      CancellationToken cancellationToken)
  {
    try
    {
      // Get the pet walker
      var spec = new GetPetWalkerPicturesSpecification(petWalkerId);
      var petWalker = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      if (petWalker == null)
      {
        return Result.NotFound($"PetWalker with ID {petWalkerId} not found");
      }

      // Save the file
      var fileUrl = await SaveFileAsync(file, petWalkerId, photoType);

      // Create a new photo entity
      var photo = new Photo(fileUrl, description)
      {
        UserId = petWalkerId,
        PhotoType = photoType
      };

      // Add the photo to the pet walker
      petWalker.Photos.Add(photo);

      // Save changes to the database
      await _repository.UpdateAsync(petWalker, cancellationToken);

      return Result.Success(photo);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error adding photo for PetWalker {PetWalkerId}", petWalkerId);
      return Result.Error($"Error adding photo: {ex.Message}");
    }
  }

  private async Task<string> SaveFileAsync(IFormFile file, Guid petWalkerId, PhotoType photoType)
  {
    // Ensure the photos directory exists
    var photosDirectory = Path.Combine(_webHostEnvironment.WebRootPath, PHOTOS_FOLDER, petWalkerId.ToString());
    Directory.CreateDirectory(photosDirectory);

    // Generate a unique filename
    var fileExtension = Path.GetExtension(file.FileName);
    var fileName = $"{Guid.NewGuid()}{fileExtension}";
    var filePath = Path.Combine(photosDirectory, fileName);

    // Save the file
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
      await file.CopyToAsync(stream);
    }

    // Return the URL (relative to the web root)
    var baseUrl = GetBaseUrl();
    var relativeUrl = $"/{PHOTOS_FOLDER}/{petWalkerId}/{fileName}";
    var fullUrl = $"{baseUrl}{relativeUrl}";
    return fullUrl;
  }

  private string GetBaseUrl()
  {
    var request = _httpContextAccessor.HttpContext?.Request;
    if (request == null)
      return string.Empty;

    return $"{request.Scheme}://{request.Host}";
  }
}
