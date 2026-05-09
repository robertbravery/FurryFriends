using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Components.Pages.Ratings;

public partial class RatingSubmission
{
    [Inject]
    public IRatingService RatingService { get; set; } = default!;

    [Inject]
    public ILogger<RatingSubmission> Logger { get; set; } = default!;

    [Parameter]
    public Guid PetWalkerId { get; set; }

    [Parameter]
    public Guid? ExistingRatingId { get; set; }

    [Parameter]
    public int? ExistingRatingValue { get; set; }

    [Parameter]
    public string? ExistingComment { get; set; }

    [Parameter]
    public bool CanEdit { get; set; } = true;

    [Parameter]
    public bool CanDelete { get; set; }

    [Parameter]
    public Guid ClientId { get; set; }

    [Parameter]
    public string HeaderText { get; set; } = "Rate Your Experience";

    [Parameter]
    public string SubmitButtonText { get; set; } = "Submit Rating";

    [Parameter]
    public bool ShowBookingField { get; set; } = true;

    [Parameter]
    public bool ShowCancelButton { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public EventCallback OnRatingSubmitted { get; set; }

    [Parameter]
    public EventCallback OnRatingDeleted { get; set; }

    public string BookingIdText { get; set; } = string.Empty;
    public int SelectedRating { get; set; }
    public string? Comment { get; set; }
    public bool IsLoading { get; set; }
    public bool IsSaved { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    private bool CommentLengthExceeded => Comment?.Length > 1000;

    protected override void OnInitialized()
    {
        if (ExistingRatingValue.HasValue)
        {
            SelectedRating = ExistingRatingValue.Value;
        }

        if (!string.IsNullOrEmpty(ExistingComment))
        {
            Comment = ExistingComment;
        }

        if (ExistingRatingId.HasValue)
        {
            ShowBookingField = false;
            HeaderText = "Edit Your Rating";
            SubmitButtonText = "Save Changes";
            CanDelete = true;
        }
    }

    public void SelectRating(int rating)
    {
        if (!CanEdit) return;
        Logger.LogInformation("Rating selected: {Rating}", rating);
        SelectedRating = rating;
    }

    public async Task SubmitRating()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        if (SelectedRating < 1 || SelectedRating > 5)
        {
            ErrorMessage = "Please select a rating between 1 and 5 stars.";
            return;
        }

        if (Comment?.Length > 1000)
        {
            ErrorMessage = "Comment must be 1000 characters or less.";
            return;
        }

        if (ExistingRatingId.HasValue)
        {
            await UpdateExistingRating();
        }
        else
        {
            await CreateNewRating();
        }
    }

    private async Task CreateNewRating()
    {
        var bookingId = Guid.Empty;

        if (ShowBookingField)
        {
            if (string.IsNullOrWhiteSpace(BookingIdText))
            {
                ErrorMessage = "Please enter a Booking ID.";
                return;
            }

            if (!Guid.TryParse(BookingIdText, out bookingId))
            {
                ErrorMessage = "Please enter a valid Booking ID.";
                return;
            }
        }

        try
        {
            IsLoading = true;
            StateHasChanged();

            var request = new CreateRatingRequest(bookingId, SelectedRating, Comment);

            Logger.LogInformation("Submitting rating for Booking: {BookingId}, Rating: {Rating}", bookingId, SelectedRating);

            var result = await RatingService.CreateRatingAsync(request);

            if (result.IsSuccess)
            {
                Logger.LogInformation("Rating submitted successfully for Booking: {BookingId}", bookingId);
                IsSaved = true;
                SuccessMessage = "Your rating has been submitted successfully!";
                await OnRatingSubmitted.InvokeAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to submit rating. Please try again.";
                Logger.LogWarning("Failed to submit rating: {Error}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting rating");
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task UpdateExistingRating()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var request = new UpdateRatingRequest(SelectedRating, Comment);

            Logger.LogInformation("Updating rating: {RatingId} with Rating: {Rating}", ExistingRatingId, SelectedRating);

            var result = await RatingService.UpdateRatingAsync(ExistingRatingId!.Value, request);

            if (result.IsSuccess)
            {
                Logger.LogInformation("Rating updated successfully: {RatingId}", ExistingRatingId);
                IsSaved = true;
                SuccessMessage = "Your rating has been updated successfully!";
                await OnRatingSubmitted.InvokeAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to update rating.";
                Logger.LogWarning("Failed to update rating: {Error}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating rating: {RatingId}", ExistingRatingId);
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    public async Task DeleteRating()
    {
        if (!ExistingRatingId.HasValue || ClientId == Guid.Empty) return;

        try
        {
            IsLoading = true;
            StateHasChanged();

            Logger.LogInformation("Deleting rating: {RatingId}", ExistingRatingId);

            var result = await RatingService.DeleteRatingAsync(ExistingRatingId.Value, ClientId);

            if (result.IsSuccess)
            {
                Logger.LogInformation("Rating deleted successfully: {RatingId}", ExistingRatingId);
                IsSaved = true;
                SuccessMessage = "Your rating has been deleted.";
                await OnRatingDeleted.InvokeAsync();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Failed to delete rating.";
                Logger.LogWarning("Failed to delete rating: {Error}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting rating: {RatingId}", ExistingRatingId);
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    public void ClearError()
    {
        ErrorMessage = null;
    }

    public void ResetForm()
    {
        BookingIdText = string.Empty;
        SelectedRating = 0;
        Comment = null;
        IsSaved = false;
        ErrorMessage = null;
        SuccessMessage = null;
        StateHasChanged();
    }

    public async Task Cancel()
    {
        ResetForm();
        await OnCancel.InvokeAsync();
    }

    private static string GetRatingLabel(int rating) => rating switch
    {
        1 => "Poor",
        2 => "Below Average",
        3 => "Average",
        4 => "Good",
        5 => "Excellent",
        _ => "Select a rating"
    };
}
