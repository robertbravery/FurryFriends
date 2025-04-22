using FurryFriends.BlazorUI.Client.Services.Interfaces;
using System;

namespace FurryFriends.BlazorUI.Client.Services.Implementation;

public class PopupService : IPopupService
{
    public event Action<string> OnShowEditClientPopup = default!;
    public event Action OnCloseEditClientPopup = default!;

    // Track the current state to handle page refreshes
    private string _currentClientEmail = string.Empty;
    private bool _isEditClientPopupOpen = false;

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
            Console.WriteLine($"Error showing popup: {ex.Message}");
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
            Console.WriteLine($"Error closing popup: {ex.Message}");
        }
    }

    public bool IsEditClientPopupOpen()
    {
        return _isEditClientPopupOpen;
    }

    public string GetCurrentClientEmail()
    {
        return _currentClientEmail;
    }
}
