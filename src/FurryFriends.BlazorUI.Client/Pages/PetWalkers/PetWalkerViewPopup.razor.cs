using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FurryFriends.BlazorUI.Client.Pages.PetWalkers;

public partial class PetWalkerViewPopup
{
    [Parameter]
    public string PetWalkerEmail { get; set; } = default!;

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Inject]
    public IPetWalkerService PetWalkerService { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    private PetWalkerDetailDto petWalkerModel = new();
    private bool isLoading = true;
    private string? loadError = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadPetWalkerData();
    }

    private async Task LoadPetWalkerData()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            var response = await PetWalkerService.GetPetWalkerDetailsByEmailAsync(PetWalkerEmail);

            if (response != null && response.Success && response.Data != null)
            {
                petWalkerModel = response.Data;
                loadError = null;
            }
            else
            {
                var errorMessage = response?.Message ?? "Failed to load pet walker data.";
                var errors = response?.Errors != null && response.Errors.Any()
                    ? string.Join(", ", response.Errors)
                    : string.Empty;

                loadError = !string.IsNullOrEmpty(errors)
                    ? $"{errorMessage}: {errors}"
                    : errorMessage;
            }
        }
        catch (Exception ex)
        {
            loadError = $"Error loading pet walker: {ex.Message}";
            Console.WriteLine($"Error loading pet walker: {ex}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private string FormatPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return string.Empty;

        // Format the phone number as needed
        return phoneNumber.Length == 10
            ? $"({phoneNumber.Substring(0, 3)}) {phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6)}"
            : phoneNumber;
    }
}
