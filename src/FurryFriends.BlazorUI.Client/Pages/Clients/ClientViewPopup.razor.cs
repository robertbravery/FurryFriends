﻿using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Models.Clients.Enums;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace FurryFriends.BlazorUI.Client.Pages.Clients;

public partial class ClientViewPopup
{
    [Parameter]
    public string ClientEmail { get; set; } = default!;

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Inject]
    public IClientService ClientService { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [Inject]
    public ILogger<ClientViewPopup> Logger { get; set; } = default!;

    private ClientModel clientModel = new();
    private PetDto[]? clientPets;
    private bool isLoading = true;
    private bool isPetsLoading = true;
    private string? loadError = null;
    // No longer need the isPetsPanelOpen flag since we're showing pets directly

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
            Logger.LogInformation("Loading client data for email: {ClientEmail}", ClientEmail);
            isLoading = true;
            isPetsLoading = true;
            StateHasChanged();

            var clientResponse = await ClientService.GetClientByEmailAsync(ClientEmail);

            if (clientResponse != null && clientResponse.Success)
            {
                Logger.LogInformation("Successfully loaded client data for email: {ClientEmail}", ClientEmail);
                clientModel = ClientData.MapToModel(clientResponse.Data);
                clientPets = clientResponse.Data.Pets;
                loadError = null;
            }
            else
            {
                loadError = "Failed to load client data.";
                Logger.LogWarning("Failed to load client data for email: {ClientEmail}", ClientEmail);
            }
        }
        catch (Exception ex)
        {
            loadError = $"Error loading client: {ex.Message}";
            Logger.LogError(ex, "Error loading client data for email: {ClientEmail}", ClientEmail);
        }
        finally
        {
            isLoading = false;
            isPetsLoading = false;
            StateHasChanged();
        }
    }

    // Get a user-friendly description of the referral source
    private string GetReferralSourceDescription(ReferralSource referralSource)
    {
        return referralSource switch
        {
            ReferralSource.None => "None",
            ReferralSource.Website => "Website",
            ReferralSource.ExistingClient => "Existing Client",
            ReferralSource.SocialMedia => "Social Media",
            ReferralSource.SearchEngine => "Search Engine",
            ReferralSource.Veterinarian => "Veterinarian",
            ReferralSource.LocalAdvertising => "Local Advertising",
            ReferralSource.Other => "Other",
            _ => referralSource.ToString()
        };
    }
}
