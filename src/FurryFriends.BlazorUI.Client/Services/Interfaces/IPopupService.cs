using System;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

public interface IPopupService
{
    // Edit popup events and methods
    event Action<string> OnShowEditClientPopup;
    event Action OnCloseEditClientPopup;

    void ShowEditClientPopup(string clientEmail);
    void CloseEditClientPopup();

    // View popup events and methods
    event Action<string> OnShowViewClientPopup;
    event Action OnCloseViewClientPopup;

    void ShowViewClientPopup(string clientEmail);
    void CloseViewClientPopup();

    // Methods to get the current state
    bool IsEditClientPopupOpen();
    bool IsViewClientPopupOpen();
    string GetCurrentClientEmail();
}
