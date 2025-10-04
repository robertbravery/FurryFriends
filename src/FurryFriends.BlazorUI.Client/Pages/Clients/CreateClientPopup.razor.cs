using FurryFriends.BlazorUI.Client.Models;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FurryFriends.BlazorUI.Client.Pages.Clients;

public partial class CreateClientPopup
{
  [Parameter]
  public EventCallback OnSave { get; set; }

  [Parameter]
  public EventCallback OnCancel { get; set; }

  [Inject]
  public IClientService ClientService { get; set; } = default!;

  [Inject]
  public IJSRuntime JS { get; set; } = default!;

  private ClientModel clientModel = new();
  private bool isSubmitting = false;
  private string? errorMessage = null;

  protected override void OnInitialized()
  {
    // Initialize a new client model
    clientModel = new ClientModel
    {
      Address = new Address(),
      IsActive = true
    };
  }

  private async Task HandleValidSubmit()
  {
    if (clientModel != null)
    {
      try
      {
        isSubmitting = true;
        StateHasChanged();

        // Create the client request DTO
        ClientRequestDto clientRequest = ClientModel.MapToRequest(clientModel);

        // Call the client service to create the client
        await ClientService.CreateClientAsync(clientRequest);

        // Invoke the OnSave callback
        await OnSave.InvokeAsync();
      }
      catch (Exception ex)
      {
        errorMessage = $"Error creating client: {ex.Message}";
        isSubmitting = false;
        StateHasChanged();
      }
    }
  }
}
