using FurryFriends.BlazorUI.Client.Models.Picture;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FurryFriends.BlazorUI.Client.Pages.PetWalkers;

public partial class ManagePetWalkerPhotosPopup : IDisposable
{
  [Inject]
  public IPopupService PopupService { get; set; } = default!;

  [Inject]
  public IPictureService PictureService { get; set; } = default!;

  [Inject]
  public IConfiguration Configuration { get; set; } = default!;

  [Inject]
  public ILogger<ManagePetWalkerPhotosPopup> Logger { get; set; } = default!;

  public bool _isVisible = false;
  public bool _isLoading = false;
  public bool _isUploadingBioPic = false;
  public bool _isUploadingGallery = false;
  public bool _isDeleting = false;
  public string? _errorMessage = null;
  public string? _bioPicUploadStatus = null;
  public string? _galleryUploadStatus = null;
  public PictureViewModel? _petWalkerPictures;
  public string? _photoBaseUrl;

  // Bio Picture State
  public IBrowserFile? _selectedBioPictureFile;
  public string? _bioPicturePreviewUrl;

  // Gallery Photos State
  public List<IBrowserFile> _selectedGalleryFiles = new();
  public List<PhotoPreview> _galleryPhotoPreviews = new();

  public class PhotoPreview
  {
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
  }

  protected override void OnInitialized()
  {
    PopupService.OnShowManagePetWalkerPhotosPopup += ShowPopup;
    PopupService.OnCloseManagePetWalkerPhotosPopup += ClosePopupInternal;
    _photoBaseUrl = Configuration["PhotoStorage:BaseUrl"]; // Get base URL
    base.OnInitialized();
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      Logger.LogDebug("ManagePetWalkerPhotosPopup first render completed");
      await Task.CompletedTask;
    }
  }

  public async void ShowPopup(Guid petWalkerId)
  {
    Logger.LogInformation("Opening manage photos popup for pet walker ID: {PetWalkerId}", petWalkerId);
    try
    {
      Logger.LogDebug("Setting visibility and loading state");
      _isVisible = true;
      _isLoading = true;
      _errorMessage = null;
      _petWalkerPictures = null; // Reset
      ResetUploadStates();

      await InvokeAsync(StateHasChanged); // Ensure UI updates immediately

      // Fetch specific pet walker details including photos
      Logger.LogDebug("Fetching pet walker photos for ID: {PetWalkerId}", petWalkerId);
      var response = await PictureService.GetPetWalkerPicturesAsync(petWalkerId);

      if (response.Success && response.Data != null)
      {
        _petWalkerPictures = response.Data;
        Logger.LogInformation("Successfully loaded pet walker pictures: Name={Name}, ProfilePic={HasProfilePic}, Photos Count={PhotoCount}",
            _petWalkerPictures.PetWalkerName,
            _petWalkerPictures.ProfilePicture != null ? "Present" : "Null",
            _petWalkerPictures.Photos?.Count.ToString() ?? "null");

        // Ensure Photos collection is initialized
        if (_petWalkerPictures.Photos == null)
        {
          Logger.LogWarning("Photos collection was null for pet walker {PetWalkerId}, initializing empty list", petWalkerId);
          _petWalkerPictures.Photos = new List<DetailedPhotoDto>();
        }
      }
      else
      {
        _errorMessage = response.Message ?? "Failed to load pet walker data.";
        Logger.LogWarning("Error loading pet walker data for ID {PetWalkerId}: {ErrorMessage}",
            petWalkerId, _errorMessage);
      }
    }
    catch (Exception ex)
    {
      _errorMessage = $"An error occurred: {ex.Message}";
      Logger.LogError(ex, "Exception in ShowPopup for pet walker ID: {PetWalkerId}", petWalkerId);
    }
    finally
    {
      _isLoading = false;
      await InvokeAsync(StateHasChanged); // Ensure UI updates after loading
    }
  }

  public void ClosePopup()
  {
    PopupService.CloseManagePetWalkerPhotosPopup();
  }

  public void ClosePopupInternal()
  {
    _isVisible = false;
    ResetUploadStates();
    _petWalkerPictures = null; // Clear data
    StateHasChanged();
  }

  public void ResetUploadStates()
  {
    _selectedBioPictureFile = null;
    _bioPicturePreviewUrl = null;
    _selectedGalleryFiles.Clear();
    _galleryPhotoPreviews.Clear();
    _bioPicUploadStatus = null;
    _galleryUploadStatus = null;
    _isUploadingBioPic = false;
    _isUploadingGallery = false;
    _isDeleting = false;
  }

  // --- Bio Picture Handlers ---
  public async Task HandleBioPictureSelected(InputFileChangeEventArgs e)
  {
    _selectedBioPictureFile = e.File;
    _bioPicUploadStatus = null; // Clear previous status
    if (_selectedBioPictureFile != null)
    {
      // Create preview
      var format = "image/jpeg"; // Or derive from file type
      var imageStream = _selectedBioPictureFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB limit example
      using var ms = new MemoryStream();
      await imageStream.CopyToAsync(ms);
      var buffer = ms.ToArray();
      _bioPicturePreviewUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
    }
    else
    {
      _bioPicturePreviewUrl = null;
    }
    StateHasChanged();
  }

  public async Task UploadBioPicture()
  {
    if (_selectedBioPictureFile == null || _petWalkerPictures == null) return;

    _isUploadingBioPic = true;
    _bioPicUploadStatus = "Uploading...";
    StateHasChanged();

    try
    {
      // Call the service method - this needs to be created
      var response = await PictureService.UpdateBioPictureAsync(_petWalkerPictures.PetWalkerId, _selectedBioPictureFile);
      if (response.Success && response.Data != null)
      {
        // Update the local model with the new bio pic URL/data
        _petWalkerPictures.ProfilePicture = response.Data;
        _bioPicUploadStatus = "Bio picture updated successfully!";
        _selectedBioPictureFile = null; // Clear selection
        _bioPicturePreviewUrl = null; // Clear preview
      }
      else
      {
        _bioPicUploadStatus = $"Error: {response.Message ?? "Upload failed."}";
      }
    }
    catch (Exception ex)
    {
      _bioPicUploadStatus = $"Error: {ex.Message}";
    }
    finally
    {
      _isUploadingBioPic = false;
      StateHasChanged();
    }
  }

  // --- Gallery Photo Handlers ---
  public async Task HandleGalleryPhotosSelected(InputFileChangeEventArgs e)
  {
    _selectedGalleryFiles.Clear();
    _galleryPhotoPreviews.Clear();
    _galleryUploadStatus = null;

    foreach (var file in e.GetMultipleFiles(maximumFileCount: 10)) // Limit concurrent uploads
    {
      try
      {
        _selectedGalleryFiles.Add(file);
        // Create previews
        var format = "image/jpeg";
        var imageStream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB limit
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        var buffer = ms.ToArray();
        _galleryPhotoPreviews.Add(new PhotoPreview { Url = $"data:{format};base64,{Convert.ToBase64String(buffer)}", Name = file.Name });
      }
      catch (IOException ex)
      {
        _galleryUploadStatus = $"Error reading {file.Name}: {ex.Message}"; // Handle file size errors etc.
      }
    }
    StateHasChanged();
  }

  public async Task UploadGalleryPhotos()
  {
    if (!_selectedGalleryFiles.Any() || _petWalkerPictures == null) return;

    _isUploadingGallery = true;
    _galleryUploadStatus = $"Uploading {_selectedGalleryFiles.Count} photos...";
    StateHasChanged();

    try
    {
      // Call the service method - needs to be created
      var response = await PictureService.AddPhotosAsync(_petWalkerPictures.PetWalkerId, _selectedGalleryFiles);
      if (response.Success && response.Data != null)
      {
        // Add new photos to the local list
        if (_petWalkerPictures.Photos == null) _petWalkerPictures.Photos = new List<DetailedPhotoDto>();
        _petWalkerPictures.Photos.AddRange(response.Data); // Assuming response returns the newly added photos
        _galleryUploadStatus = $"{response.Data.Count} photo(s) uploaded successfully!";
        _selectedGalleryFiles.Clear(); // Clear selection
        _galleryPhotoPreviews.Clear(); // Clear previews
      }
      else
      {
        _galleryUploadStatus = $"Error: {response.Message ?? "Upload failed."}";
      }
    }
    catch (Exception ex)
    {
      _galleryUploadStatus = $"Error: {ex.Message}";
    }
    finally
    {
      _isUploadingGallery = false;
      StateHasChanged();
    }
  }

  public async Task DeleteGalleryPhoto(Guid photoId)
  {
    if (_petWalkerPictures == null) return;

    // Optional: Add a confirmation dialog here

    _isDeleting = true;
    _galleryUploadStatus = "Deleting photo..."; // Use gallery status for feedback
    StateHasChanged();

    try
    {
      // Call the service method - needs to be created
      var response = await PictureService.DeletePhotoAsync(_petWalkerPictures.PetWalkerId, photoId);
      if (response.Success)
      {
        // Remove photo from local list
        _petWalkerPictures.Photos?.RemoveAll(p => p.Id == photoId);
        _galleryUploadStatus = "Photo deleted successfully!";
      }
      else
      {
        _galleryUploadStatus = $"Error: {response.Message ?? "Deletion failed."}";
      }
    }
    catch (Exception ex)
    {
      _galleryUploadStatus = $"Error: {ex.Message}";
    }
    finally
    {
      _isDeleting = false;
      StateHasChanged();
    }
  }

  // Helper to construct full URL
  public string GetFullPhotoUrl(string relativePath)
  {
    if (string.IsNullOrEmpty(relativePath)) return ""; // Or return placeholder
    if (Uri.TryCreate(relativePath, UriKind.Absolute, out _)) return relativePath; // Already absolute
    if (string.IsNullOrEmpty(_photoBaseUrl)) return relativePath; // No base URL configured

    // Ensure no double slashes
    var baseUrl = _photoBaseUrl.TrimEnd('/');
    var relPath = relativePath.TrimStart('/');
    return $"{baseUrl}/{relPath}";
  }

  public void Dispose()
  {
    PopupService.OnShowManagePetWalkerPhotosPopup -= ShowPopup;
    PopupService.OnCloseManagePetWalkerPhotosPopup -= ClosePopupInternal;
  }
}

