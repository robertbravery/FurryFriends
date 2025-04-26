using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Pages.PetWalkers;

public partial class EditPetWalkerPopup
{
  [Parameter]
  public string PetWalkerEmail { get; set; } = default!;

  [Inject]
  public IPetWalkerService PetWalkerService { get; set; } = default!;

  [Inject]
  public IPopupService PopupService { get; set; } = default!;

  private PetWalkerDetailDto? petWalkerModel;
  private bool isLoading = true;
  private string? loadError;
  private bool showSuccessMessage = false;

  protected override async Task OnInitializedAsync()
  {
    await LoadPetWalker();
  }

  private async Task LoadPetWalker()
  {
    try
    {
      isLoading = true;
      var result = await PetWalkerService.GetPetWalkerDetailsByEmailAsync(PetWalkerEmail);
      
      if (result.Success && result.Data != null)
      {
        petWalkerModel = result.Data;
        
        // Initialize service areas if null
        if (petWalkerModel.ServiceAreas == null)
        {
          petWalkerModel.ServiceAreas = new List<string>();
        }
        // If service areas come as a single string, split them
        else if (petWalkerModel.ServiceAreas.Count == 1 && petWalkerModel.ServiceAreas[0].Contains(","))
        {
          var areas = petWalkerModel.ServiceAreas[0]
            .Split(',')
            .Select(a => a.Trim())
            .Where(a => !string.IsNullOrEmpty(a))
            .ToList();
          petWalkerModel.ServiceAreas = areas;
        }
      }
      else
      {
        loadError = result.Message ?? "Failed to load pet walker details.";
      }
    }
    catch (Exception ex)
    {
      loadError = $"Error: {ex.Message}";
    }
    finally
    {
      isLoading = false;
    }
  }

  private async Task HandleValidSubmit()
  {
    try 
    {
      var result = await PetWalkerService.UpdatePetWalkerAsync(petWalkerModel!);
      if (result.Success)
      {
        showSuccessMessage = true;
        StateHasChanged();
        
        // Auto-close the popup after 2 seconds
        await Task.Delay(2000);
        PopupService.CloseEditPetWalkerPopup();
      }
      else
      {
        loadError = result.Message ?? "Failed to update pet walker.";
        StateHasChanged();
      }
    }
    catch (Exception ex)
    {
      loadError = $"Error: {ex.Message}";
      StateHasChanged();
    }
  }

  private void OnCancel()
  {
    PopupService.CloseEditPetWalkerPopup();
  }

  private void AddServiceArea()
  {
    if (petWalkerModel!.ServiceAreas.Count < 10) // Limit to 10 service areas
    {
      petWalkerModel.ServiceAreas.Add(string.Empty);
      StateHasChanged();
    }
  }

  private void RemoveServiceArea(int index)
  {
    if (index >= 0 && index < petWalkerModel!.ServiceAreas.Count)
    {
      petWalkerModel.ServiceAreas.RemoveAt(index);
      StateHasChanged();
    }
  }
}
