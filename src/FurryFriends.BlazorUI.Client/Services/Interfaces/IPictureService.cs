using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.Picture;
using Microsoft.AspNetCore.Components.Forms;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

public interface IPictureService
{
  ///<summary>
  /// New methods for photo management
  /// </summary>
  Task<ApiResponse<DetailedPhotoDto>> UpdateBioPictureAsync(Guid petWalkerId, IBrowserFile file);
  Task<ApiResponse<DetailedPhotoDto>> AddBioPictureAsync(Guid petWalkerId, IBrowserFile file);
  Task<ApiResponse<List<DetailedPhotoDto>>> AddPhotosAsync(Guid petWalkerId, IEnumerable<IBrowserFile> files);
  Task<ApiResponse<bool>> DeletePhotoAsync(Guid petWalkerId, Guid photoId);
  Task<ApiResponse<PictureViewModel>> GetPetWalkerPicturesAsync(Guid petWalkerId);
}
