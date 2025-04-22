using System;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

public interface IPopupService
{
    event Action<string> OnShowEditClientPopup;
    event Action OnCloseEditClientPopup;

    void ShowEditClientPopup(string clientEmail);
    void CloseEditClientPopup();

    // Methods to get the current state
    bool IsEditClientPopupOpen();
    string GetCurrentClientEmail();
}
