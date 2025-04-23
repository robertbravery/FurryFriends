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

    // Track the current state to handle page refreshes
    private string _currentClientEmail = string.Empty;
    private bool _isEditClientPopupOpen = false;
    private bool _isViewClientPopupOpen = false;
    private bool _isCreateClientPopupOpen = false;

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

    public string GetCurrentClientEmail()
    {
        return _currentClientEmail;
    }
}
