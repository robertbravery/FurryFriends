using FurryFriends.BlazorUI.Client.Models;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace FurryFriends.BlazorUI.Client.Pages.Clients;

public partial class CreateClient
{
  [SupplyParameterFromForm]
  private ClientModel clientModel { get; set; } = new()
  {
    Address = new Address() // Ensure Address is initialized
  };
  private bool isSubmitting = false;
  private string? errorMessage;

  [Inject]
  public IClientService ClientService { get; set; } = default!;

  [Inject]
  public NavigationManager NavigationManager { get; set; } = default!;

  [Inject]
  public ILogger<CreateClient> Logger { get; set; } = default!;

  protected override void OnInitialized()
  {
    clientModel ??= new ClientModel();

    if (clientModel.Address is null)
    {
      clientModel.Address = new Address();
    }
    isSubmitting = false;
  }

  private async Task HandleSaveClient(EditContext editContext)
  {
    Logger.LogInformation("Saving client");
    errorMessage = null;

    // Manually validate the form
    var isValid = editContext.Validate();

    // If validation fails, don't proceed with submission
    if (!isValid)
    {
      return;
    }

    try
    {
      isSubmitting = true;
      var clientRequest = ClientRequestDto.MapToDto(clientModel);
      await ClientService.CreateClientAsync(clientRequest);
      clientModel = new ClientModel();
      NavigationManager.NavigateTo("clients");
    }
    catch (HttpRequestException ex)
    {
      errorMessage = $"Error saving client: {ex.Message}";
    }
    catch (Exception ex)
    {
      errorMessage = $"An unexpected error occurred: {ex.Message}";
    }
    finally
    {
      isSubmitting = false;
    }
  }

  private void HandleCancel()
  {
    Logger.LogInformation("Navigating to clients page");
    clientModel = new ClientModel();
    NavigationManager.NavigateTo("clients");
  }

  private void NavigateToAbsoluteURL(MouseEventArgs args)
  {
    Logger.LogInformation("Navigating to {url}", args);
  }
}
