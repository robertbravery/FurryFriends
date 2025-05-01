using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

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

  [Inject]
  public ILogger<EditPetPopup> Logger { get; set; } = default!;

  private bool isLoading = false;
  private List<BreedDto>? breeds;

  protected override async Task OnInitializedAsync()
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
        BreedId = 0, // Will be set after loading breeds
        Age = Pet.Age,
        Weight = Pet.Weight,
        SpecialNeeds = Pet.SpecialNeeds,
        MedicalConditions = Pet.MedicalConditions,
        isActive = Pet.isActive,
        Photo = Pet.Photo
      };
    }

    // Load breeds
    try
    {
      Logger.LogInformation("Loading breeds for pet: {PetName}", Pet?.Name);
      breeds = await ClientService.GetBreedsAsync();
      Logger.LogDebug("Loaded {BreedCount} breeds", breeds?.Count);

      // If we have the breed name, try to find the corresponding BreedId
      if (!string.IsNullOrEmpty(Pet?.Breed) && breeds != null)
      {
        var matchingBreed = breeds.FirstOrDefault(b =>
            b.Name.Equals(Pet.Breed, StringComparison.OrdinalIgnoreCase));

        if (matchingBreed != null)
        {
          Logger.LogDebug("Found matching breed ID {BreedId} for breed name {BreedName}",
            matchingBreed.Id, Pet.Breed);
          Pet.BreedId = matchingBreed.Id;
        }
        else
        {
          Logger.LogWarning("No matching breed found for breed name {BreedName}", Pet.Breed);
        }
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error loading breeds for pet: {PetName}", Pet?.Name);
    }
  }

  private async Task HandleValidSubmit()
  {
    // Set the Breed name based on the selected BreedId
    if (Pet.BreedId > 0 && breeds != null)
    {
      var selectedBreed = breeds.FirstOrDefault(b => b.Id == Pet.BreedId);
      if (selectedBreed != null)
      {
        Pet.Breed = selectedBreed.Name;
      }
    }

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
              Logger.LogError(ex, "Error updating pet: {PetName} for client: {ClientEmail}",
                Pet.Name, ClientEmail);
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
