using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Client.Pages.Rating;

public partial class RatingSubmission : ComponentBase
{
    [Inject]
    public IRatingService RatingService { get; set; } = default!;

    [Inject]
    public ILogger<RatingSubmission> Logger { get; set; } = default!;

    [Parameter]
    public Guid PetWalkerId { get; set; }

    [Parameter]
    public Guid ClientId { get; set; }

    [Parameter]
    public EventCallback OnRatingSubmitted { get; set; }

    public int SelectedRating { get; set; }

    public string? Comment { get; set; }
    public bool isLoading { get; set; }
    public bool hasSubmitted { get; set; }
    public string? errorMessage { get; set; }

    public void SelectRating(int rating)
    {
        Logger.LogInformation("SelectRating called with rating: {Rating}", rating);
        SelectedRating = rating;
        StateHasChanged();
    }

    public async Task SubmitRating()
    {
        errorMessage = null;

        if (SelectedRating < 1 || SelectedRating > 5)
        {
            errorMessage = "Please select a rating between 1 and 5 stars.";
            return;
        }

        try
        {
            isLoading = true;
            await InvokeAsync(StateHasChanged);

            Logger.LogInformation("Submitting rating for PetWalker: {PetWalkerId}, Rating: {Rating}",
                PetWalkerId, SelectedRating);

            var request = new CreateRatingRequest(PetWalkerId, ClientId, SelectedRating, Comment);
            var result = await RatingService.CreateRatingAsync(request);

            if (result.IsSuccess)
            {
                Logger.LogInformation("Rating submitted successfully for PetWalker: {PetWalkerId}", PetWalkerId);
                hasSubmitted = true;
                await OnRatingSubmitted.InvokeAsync();
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "Failed to submit rating. Please try again.";
                Logger.LogWarning("Failed to submit rating for PetWalker: {PetWalkerId}", PetWalkerId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting rating for PetWalker: {PetWalkerId}", PetWalkerId);
            errorMessage = $"Error submitting rating: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    public void ResetForm()
    {
        SelectedRating = 0;
        Comment = null;
        hasSubmitted = false;
        errorMessage = null;
        StateHasChanged();
    }
}
