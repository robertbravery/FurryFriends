using FurryFriends.BlazorUI.Client.Services.Interfaces;
using System;

namespace FurryFriends.BlazorUI.Client.Services.Implementation;

public class PopupService : IPopupService
{
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
            OnShowEditClientPopup?.Invoke(clientEmail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing edit popup: {ex.Message}");
        }
    }

    public void CloseEditClientPopup()
    {
        _isEditClientPopupOpen = false;

        try
        {
            OnCloseEditClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing edit popup: {ex.Message}");
        }
    }

    // View popup methods
    public void ShowViewClientPopup(string clientEmail)
    {
        _currentClientEmail = clientEmail;
        _isViewClientPopupOpen = true;

        try
        {
            OnShowViewClientPopup?.Invoke(clientEmail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing view popup: {ex.Message}");
        }
    }

    public void CloseViewClientPopup()
    {
        _isViewClientPopupOpen = false;

        try
        {
            OnCloseViewClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing view popup: {ex.Message}");
        }
    }

    // Create popup methods
    public void ShowCreateClientPopup()
    {
        _isCreateClientPopupOpen = true;

        try
        {
            OnShowCreateClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing create popup: {ex.Message}");
        }
    }

    public void CloseCreateClientPopup()
    {
        _isCreateClientPopupOpen = false;

        try
        {
            OnCloseCreateClientPopup?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing create popup: {ex.Message}");
        }
    }

    // PetWalker view popup methods
    public void ShowViewPetWalkerPopup(string petWalkerEmail)
    {
        _currentPetWalkerEmail = petWalkerEmail;
        _isViewPetWalkerPopupOpen = true;

        try
        {
            OnShowViewPetWalkerPopup?.Invoke(petWalkerEmail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing pet walker view popup: {ex.Message}");
        }
    }

    public void CloseViewPetWalkerPopup()
    {
        _isViewPetWalkerPopupOpen = false;

        try
        {
            OnCloseViewPetWalkerPopup?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing pet walker view popup: {ex.Message}");
        }
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
            OnShowEditPetWalkerPopup?.Invoke(petWalkerEmail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing edit pet walker popup: {ex.Message}");
        }
    }

    public void CloseEditPetWalkerPopup()
    {
        _isEditPetWalkerPopupOpen = false;

        try
        {
            OnCloseEditPetWalkerPopup?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing edit pet walker popup: {ex.Message}");
        }
    }

    public bool IsEditPetWalkerPopupOpen()
    {
        return _isEditPetWalkerPopupOpen;
    }
}
