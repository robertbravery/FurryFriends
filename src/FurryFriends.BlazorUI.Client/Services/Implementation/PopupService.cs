using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace FurryFriends.BlazorUI.Client.Services.Implementation;

public class PopupService : IPopupService
{
    private readonly ILogger<PopupService> _logger;

    public PopupService(ILogger<PopupService> logger)
    {
        _logger = logger;
    }

    // Edit popup events
    public event Action<string> OnShowEditClientPopup = default!;
    public event Action OnCloseEditClientPopup = default!;

    // View popup events
    public event Action<string> OnShowViewClientPopup = default!;
    public event Action OnCloseViewClientPopup = default!;

    // Create popup events
    public event Action OnShowCreateClientPopup = default!;
    public event Action OnCloseCreateClientPopup = default!;

    // Edit PetWalker popup events
    public event Action<string> OnShowEditPetWalkerPopup = default!;
    public event Action OnCloseEditPetWalkerPopup = default!;

    // PetWalker view popup events
    public event Action<string> OnShowViewPetWalkerPopup = default!;
    public event Action OnCloseViewPetWalkerPopup = default!;

    // Manage PetWalker Photos popup events
    public event Action<Guid>? OnShowManagePetWalkerPhotosPopup;
    public event Action? OnCloseManagePetWalkerPhotosPopup;
    public Guid CurrentPetWalkerIdForPhotos { get; private set; } // Store the ID




    // Track the current state to handle page refreshes
    private string _currentClientEmail = string.Empty;
    private string _currentPetWalkerEmail = string.Empty;
    private string _currentEditPetWalkerEmail = string.Empty;
    private bool _isEditClientPopupOpen = false;
    private bool _isViewClientPopupOpen = false;
    private bool _isCreateClientPopupOpen = false;
    private bool _isEditPetWalkerPopupOpen = false;
    private bool _isViewPetWalkerPopupOpen = false;

    // Edit popup methods
    public void ShowEditClientPopup(string clientEmail)
    {
        _currentClientEmail = clientEmail;
        _isEditClientPopupOpen = true;

        try
        {
            _logger.LogDebug("Showing edit client popup for client: {ClientEmail}", clientEmail);
            OnShowEditClientPopup?.Invoke(clientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing edit client popup for client: {ClientEmail}", clientEmail);
        }
    }

    public void CloseEditClientPopup()
    {
        _isEditClientPopupOpen = false;

        try
        {
            _logger.LogDebug("Closing edit client popup");
            OnCloseEditClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing edit client popup");
        }
    }

    // View popup methods
    public void ShowViewClientPopup(string clientEmail)
    {
        _currentClientEmail = clientEmail;
        _isViewClientPopupOpen = true;

        try
        {
            _logger.LogDebug("Showing view client popup for client: {ClientEmail}", clientEmail);
            OnShowViewClientPopup?.Invoke(clientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing view client popup for client: {ClientEmail}", clientEmail);
        }
    }

    public void CloseViewClientPopup()
    {
        _isViewClientPopupOpen = false;

        try
        {
            _logger.LogDebug("Closing view client popup");
            OnCloseViewClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing view client popup");
        }
    }

    // Create popup methods
    public void ShowCreateClientPopup()
    {
        _isCreateClientPopupOpen = true;

        try
        {
            _logger.LogDebug("Showing create client popup");
            OnShowCreateClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing create client popup");
        }
    }

    public void CloseCreateClientPopup()
    {
        _isCreateClientPopupOpen = false;

        try
        {
            _logger.LogDebug("Closing create client popup");
            OnCloseCreateClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing create client popup");
        }
    }

    // PetWalker view popup methods
    public void ShowViewPetWalkerPopup(string petWalkerEmail)
    {
        _currentPetWalkerEmail = petWalkerEmail;
        _isViewPetWalkerPopupOpen = true;

        try
        {
            _logger.LogDebug("Showing pet walker view popup for: {PetWalkerEmail}", petWalkerEmail);
            OnShowViewPetWalkerPopup?.Invoke(petWalkerEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing pet walker view popup for: {PetWalkerEmail}", petWalkerEmail);
        }
    }

    public void CloseViewPetWalkerPopup()
    {
        _isViewPetWalkerPopupOpen = false;

        try
        {
            _logger.LogDebug("Closing pet walker view popup");
            OnCloseViewPetWalkerPopup?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing pet walker view popup");
        }
    }

    public void ShowManagePetWalkerPhotosPopup(Guid petWalkerId)
    {
        try
        {
            _logger.LogDebug("Showing manage pet walker photos popup for ID: {PetWalkerId}", petWalkerId);
            CurrentPetWalkerIdForPhotos = petWalkerId;

            if (OnShowManagePetWalkerPhotosPopup != null)
            {
                _logger.LogDebug("OnShowManagePetWalkerPhotosPopup has subscribers");
                OnShowManagePetWalkerPhotosPopup.Invoke(petWalkerId);
            }
            else
            {
                _logger.LogWarning("OnShowManagePetWalkerPhotosPopup has no subscribers");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing manage pet walker photos popup for ID: {PetWalkerId}", petWalkerId);
        }
    }

    public void CloseManagePetWalkerPhotosPopup()
    {
        _logger.LogDebug("Closing manage pet walker photos popup");
        OnCloseManagePetWalkerPhotosPopup?.Invoke();
    }

    // State methods
    public bool IsEditClientPopupOpen()
    {
        return _isEditClientPopupOpen;
    }

    public bool IsViewClientPopupOpen()
    {
        return _isViewClientPopupOpen;
    }

    public bool IsCreateClientPopupOpen()
    {
        return _isCreateClientPopupOpen;
    }

    public bool IsViewPetWalkerPopupOpen()
    {
        return _isViewPetWalkerPopupOpen;
    }

    public string GetCurrentClientEmail()
    {
        return _currentClientEmail;
    }

    public string GetCurrentPetWalkerEmail()
    {
        return _currentPetWalkerEmail;
    }

    public string GetCurrentEditPetWalkerEmail()
    {
        return _currentEditPetWalkerEmail;
    }

    // Edit PetWalker popup methods
    public void ShowEditPetWalkerPopup(string petWalkerEmail)
    {
        _currentEditPetWalkerEmail = petWalkerEmail;
        _isEditPetWalkerPopupOpen = true;

        try
        {
            _logger.LogDebug("Showing edit pet walker popup for: {PetWalkerEmail}", petWalkerEmail);
            OnShowEditPetWalkerPopup?.Invoke(petWalkerEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing edit pet walker popup for: {PetWalkerEmail}", petWalkerEmail);
        }
    }

    public void CloseEditPetWalkerPopup()
    {
        _isEditPetWalkerPopupOpen = false;

        try
        {
            _logger.LogDebug("Closing edit pet walker popup");
            OnCloseEditPetWalkerPopup?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing edit pet walker popup");
        }
    }

    public bool IsEditPetWalkerPopupOpen()
    {
        return _isEditPetWalkerPopupOpen;
    }
}
