using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Pages.Clients;

public partial class AddPetPopup
{
  [Parameter]
  public Guid ClientId { get; set; }

  [Parameter]
  public string ClientEmail { get; set; } = default!;

  [Parameter]
  public EventCallback<PetDto> OnSave { get; set; }

  [Parameter]
  public EventCallback OnCancel { get; set; }

  [Inject]
  public IClientService ClientService { get; set; } = default!;

  private PetDto Pet { get; set; } = new PetDto();
  private bool isSubmitting = false;
  private string? errorMessage;
  private List<BreedDto>? breeds;

  protected override async Task OnInitializedAsync()
  {
    // Initialize a new pet with default values
    Pet = new PetDto
    {
      Id = Guid.NewGuid(),
      Name = string.Empty,
      Species = "Dog", // Default to Dog
      Breed = string.Empty,
      BreedId = 0,
      Age = 0,
      Weight = 0,
      SpecialNeeds = string.Empty,
      MedicalConditions = string.Empty,
      isActive = true,
      Photo = string.Empty
    };

    // Load breeds
    try
    {
      breeds = await ClientService.GetBreedsAsync();
    }
    catch (Exception ex)
    {
      errorMessage = $"Error loading breeds: {ex.Message}";
    }
  }

  private async Task HandleValidSubmit()
  {
    try
    {
      isSubmitting = true;
      errorMessage = null;
      StateHasChanged();

      // Set the Breed name based on the selected BreedId
      if (Pet.BreedId > 0 && breeds != null)
      {
        var selectedBreed = breeds.FirstOrDefault(b => b.Id == Pet.BreedId);
        if (selectedBreed != null)
        {
          Pet.Breed = selectedBreed.Name;
        }
      }

      // Validate that a breed is selected
      if (Pet.BreedId <= 0)
      {
        errorMessage = "Please select a valid breed";
        isSubmitting = false;
        StateHasChanged();
        return;
      }

      // Call the service to add the pet
      var petId = await ClientService.AddPetAsync(ClientId, Pet);

      // Update the pet's ID with the one returned from the API
      Pet.Id = petId;

      // Notify the parent component that the pet was added
      if (OnSave.HasDelegate)
      {
        await OnSave.InvokeAsync(Pet);
      }
    }
    catch (Exception ex)
    {
      errorMessage = $"Error adding pet: {ex.Message}";
    }
    finally
    {
      isSubmitting = false;
      StateHasChanged();
    }
  }

  private async Task HandleCancel()
  {
    if (OnCancel.HasDelegate)
    {
      await OnCancel.InvokeAsync();
    }
  }

  private void SpeciesChanged(ChangeEventArgs e)
  {
    // Reset the breed ID when species changes
    Pet.BreedId = 0;
    Pet.Breed = string.Empty;
    StateHasChanged();
  }
}
