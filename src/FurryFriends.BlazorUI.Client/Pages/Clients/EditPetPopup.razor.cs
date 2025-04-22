using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Pages.Clients;

public partial class EditPetPopup
{
    [Parameter]
    public Pet Pet { get; set; } = default!;

    [Parameter]
    public string ClientEmail { get; set; } = default!;

    [Parameter]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Inject]
    public IClientService ClientService { get; set; } = default!;

    private bool isLoading = false;

    protected override void OnInitialized()
    {
        // Make a copy of the pet to avoid modifying the original directly
        if (Pet != null)
        {
            Pet = new Pet
            {
                Id = Pet.Id,
                Name = Pet.Name,
                Species = Pet.Species,
                Breed = Pet.Breed,
                Age = Pet.Age,
                Weight = Pet.Weight,
                SpecialNeeds = Pet.SpecialNeeds,
                MedicalConditions = Pet.MedicalConditions,
                isActive = Pet.isActive,
                Photo = Pet.Photo
            };
        }
    }

    private async Task HandleValidSubmit()
    {
        // Use Task.Run to ensure the UI thread is not blocked
        await Task.Run(async () =>
        {
            // Small delay to ensure the event completes
            await Task.Delay(10);

            // Update the state on the UI thread
            await InvokeAsync(async () =>
            {
                isLoading = true;
                try
                {
                    await ClientService.UpdatePetAsync(ClientEmail, Pet);
                    await OnSave.InvokeAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating pet: {ex.Message}");
                    // Handle error (could add error state and display to user)
                }
                finally
                {
                    isLoading = false;
                }
            });
        });
    }
}
