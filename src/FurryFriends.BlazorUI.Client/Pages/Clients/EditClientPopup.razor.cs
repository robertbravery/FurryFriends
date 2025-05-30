using FurryFriends.BlazorUI.Client.Models.Clients;
using System.ComponentModel.DataAnnotations;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using FurryFriends.BlazorUI.Client.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FurryFriends.BlazorUI.Client.Pages.Clients;

    public partial class EditClientPopup
    {
        [Parameter]
        public string ClientEmail { get; set; } = default!;

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Inject]
        public IClientService ClientService { get; set; } = default!;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        private ClientModel clientModel = new();
        private Pet[]? clientPets;
        private bool isLoading = true;
        private bool isPetsLoading = true;
        private string? loadError = null;
        private bool isPetsPanelOpen = false; // Controls the visibility of the pets panel
        private bool showEditPetPopup = false; // Controls the visibility of the edit pet popup
        private Pet? selectedPet = null; // The pet being edited

        protected override async Task OnInitializedAsync()
        {
            await LoadClientData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Add Font Awesome if not already loaded
                await JS.InvokeVoidAsync("eval", @"
                    if (!document.querySelector('link[href*=""fontawesome""]')) {
                        var link = document.createElement('link');
                        link.rel = 'stylesheet';
                        link.href = 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css';
                        document.head.appendChild(link);
                    }
                ");
            }
        }

        private async Task LoadClientData()
        {
            try
            {
                isLoading = true;
                isPetsLoading = true;
                loadError = null;

                // Get client model by ID from the service
                var client = await ClientService.GetClientByEmailAsync(ClientEmail);

                if (!client.Success || client.Data == null)
                {
                    loadError = "Client not found";
                }
                else
                {
                    clientModel = ClientData.MapToModel(client.Data);
                    isLoading = false;
                    StateHasChanged();

                    // Load pets separately to allow the client form to display while pets are loading
                    try
                    {
                        clientPets = client.Data.Pets;
                        if (clientPets != null)
                        {
                            foreach (var pet in clientPets)
                            {
                                pet.Photo = await ClientService.GetDogImageAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading pet images: {ex}");
                        // We don't set loadError here since the client data loaded successfully
                    }
                    finally
                    {
                        isPetsLoading = false;
                        StateHasChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                loadError = ex.Message;
                Console.WriteLine($"Error loading client: {ex}");
            }
            finally
            {
                isLoading = false;
                isPetsLoading = false;
                StateHasChanged();
            }
        }

        private async Task HandleValidSubmit()
        {
            if (clientModel != null)
            {
                // Use Task.Run to ensure the UI thread is not blocked
                await Task.Run(async () =>
                {
                    // Small delay to ensure the event completes
                    await Task.Delay(10);

                    // Update the state on the UI thread
                    await InvokeAsync(async () =>
                    {
                        ClientRequestDto clientRequest = ClientModel.MapToRequest(clientModel);
                        await ClientService.UpdateClientAsync(clientRequest);
                        await OnSave.InvokeAsync();
                    });
                });
            }
        }

        [Parameter]
        public EventCallback<Guid> OnAddPet { get; set; }

        // Handle the add pet request from the PetsDisplay component
        private async Task HandleAddPet(string clientEmail)
        {
            // If the client has an ID, we can use it to add a pet
            if (clientModel != null && !string.IsNullOrEmpty(clientEmail))
            {
                // Use Task.Run to ensure the UI thread is not blocked
                await Task.Run(async () =>
                {
                    // Small delay to ensure the event completes
                    await Task.Delay(10);

                    // Update the state on the UI thread
                    await InvokeAsync(async () =>
                    {
                        // For now, we'll just show a JavaScript alert
                        // In a real implementation, you would navigate to a pet creation form or show a popup
                        await JS.InvokeVoidAsync("alert", $"Add a new pet for client {clientEmail}. This would open a pet creation form.");

                        // If you have a dedicated pet creation component, you could invoke it here
                        // await OnAddPet.InvokeAsync(clientData.Id);
                    });
                });
            }
        }

        // Handle the edit pet request from the PetsDisplay component
        private void HandleEditPet(Pet pet)
        {
            // Use Task.Run to ensure the UI thread is not blocked
            Task.Run(async () =>
            {
                // Small delay to ensure the event completes
                await Task.Delay(10);

                // Update the state on the UI thread
                await InvokeAsync(() =>
                {
                    selectedPet = pet;
                    showEditPetPopup = true;
                    StateHasChanged();
                });
            });
        }

        // Handle the save event from the EditPetPopup component
        private async Task HandlePetSaved()
        {
            // Use Task.Run to ensure the UI thread is not blocked
            await Task.Run(async () =>
            {
                // Small delay to ensure the event completes
                await Task.Delay(10);

                // Update the state on the UI thread
                await InvokeAsync(async () =>
                {
                    showEditPetPopup = false;
                    selectedPet = null;

                    // Reload the client data to refresh the pet list
                    await LoadClientData();
                    StateHasChanged();
                });
            });
        }

        // Handle the cancel event from the EditPetPopup component
        private void HandlePetEditCancel()
        {
            // Use Task.Run to ensure the UI thread is not blocked
            Task.Run(async () =>
            {
                // Small delay to ensure the event completes
                await Task.Delay(10);

                // Update the state on the UI thread
                await InvokeAsync(() =>
                {
                    showEditPetPopup = false;
                    selectedPet = null;
                    StateHasChanged();
                });
            });
        }

        // Toggle the visibility of the pets panel
        private void TogglePetsPanel()
        {
            // Use Task.Run to ensure the UI thread is not blocked
            Task.Run(async () =>
            {
                // Small delay to ensure the event completes
                await Task.Delay(10);

                // Update the state on the UI thread
                await InvokeAsync(() =>
                {
                    isPetsPanelOpen = !isPetsPanelOpen;
                    StateHasChanged();
                });
            });
        }
    }

