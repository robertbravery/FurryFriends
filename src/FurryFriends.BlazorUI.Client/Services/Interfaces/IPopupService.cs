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

    // Create popup events and methods
    event Action OnShowCreateClientPopup;
    event Action OnCloseCreateClientPopup;

    void ShowCreateClientPopup();
    void CloseCreateClientPopup();

    // PetWalker view popup events and methods
    event Action<string> OnShowViewPetWalkerPopup;
    event Action OnCloseViewPetWalkerPopup;

    void ShowViewPetWalkerPopup(string petWalkerEmail);
    void CloseViewPetWalkerPopup();

    // Methods to get the current state
    bool IsEditClientPopupOpen();
    bool IsViewClientPopupOpen();
    bool IsCreateClientPopupOpen();
    bool IsViewPetWalkerPopupOpen();
    string GetCurrentClientEmail();
    string GetCurrentPetWalkerEmail();
}
