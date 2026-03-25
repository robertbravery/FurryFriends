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

    // Use string for binding - parse to Guid on submit
    public string BookingIdText { get; set; } = string.Empty;
    
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

        if (string.IsNullOrWhiteSpace(BookingIdText) || !Guid.TryParse(BookingIdText, out var bookingId))
        {
            errorMessage = "Please enter a valid Booking ID.";
            return;
        }

        if (SelectedRating < 1 || SelectedRating > 5)
        {
            errorMessage = "Please select a rating between 1 and 5 stars.";
            return;
        }

        try
        {
            isLoading = true;
            await InvokeAsync(StateHasChanged);

            Logger.LogInformation("Submitting rating for Booking: {BookingId}, Rating: {Rating}",
                bookingId, SelectedRating);

            var request = new CreateRatingRequest(bookingId, SelectedRating, Comment);
            var success = await RatingService.CreateRatingAsync(request);

            if (success)
            {
                Logger.LogInformation("Rating submitted successfully for Booking: {BookingId}", bookingId);
                hasSubmitted = true;
            }
            else
            {
                errorMessage = "Failed to submit rating. Please check your booking ID and try again.";
                Logger.LogWarning("Failed to submit rating for Booking: {BookingId}", bookingId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting rating for Booking: {BookingId}", bookingId);
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
        BookingIdText = string.Empty;
        SelectedRating = 0;
        Comment = null;
        hasSubmitted = false;
        errorMessage = null;
        StateHasChanged();
    }
}
