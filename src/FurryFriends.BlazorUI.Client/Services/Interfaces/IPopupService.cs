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
    void ShowViewPetWalkerPopup(string petWalkerEmail);
    void CloseViewPetWalkerPopup();
    event Action<string> OnShowViewPetWalkerPopup;
    event Action OnCloseViewPetWalkerPopup;

    // Edit PetWalker popup events
    event Action<string> OnShowEditPetWalkerPopup;
    event Action OnCloseEditPetWalkerPopup;

    void ShowEditPetWalkerPopup(string petWalkerEmail);
    void CloseEditPetWalkerPopup();

    event Action<Guid> OnShowManagePetWalkerPhotosPopup;
    event Action OnCloseManagePetWalkerPhotosPopup;
    void ShowManagePetWalkerPhotosPopup(Guid petWalkerId);
    void CloseManagePetWalkerPhotosPopup();

    // Methods to get the current state
    bool IsEditClientPopupOpen();
    bool IsViewClientPopupOpen();
    bool IsCreateClientPopupOpen();
    bool IsViewPetWalkerPopupOpen();
    string GetCurrentClientEmail();
    string GetCurrentPetWalkerEmail();
    string GetCurrentEditPetWalkerEmail();
    bool IsEditPetWalkerPopupOpen();
}
