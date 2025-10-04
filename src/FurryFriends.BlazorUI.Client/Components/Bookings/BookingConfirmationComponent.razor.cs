using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.Clients;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Components.Bookings;

public partial class BookingConfirmationComponent
{
  [Inject] public ILogger<BookingConfirmationComponent> Logger { get; set; } = default!;

  [Parameter] public BookingRequestDto? BookingRequest { get; set; }
  [Parameter] public PetWalkerSummaryDto? SelectedPetWalker { get; set; }
  [Parameter] public PetDto? SelectedPet { get; set; }
  [Parameter] public bool IsSubmitting { get; set; }
  [Parameter] public EventCallback OnConfirm { get; set; }
  [Parameter] public EventCallback OnEdit { get; set; }

  private async Task ConfirmBooking()
  {
    try
    {
      Logger.LogInformation("User confirmed booking for PetWalker: {PetWalkerId}",
          SelectedPetWalker?.Id);

      await OnConfirm.InvokeAsync();
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error confirming booking");
    }
  }

  private async Task EditBooking()
  {
    try
    {
      Logger.LogInformation("User requested to edit booking");
      await OnEdit.InvokeAsync();
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error handling edit booking request");
    }
  }

  private string GetDurationDisplay()
  {
    if (BookingRequest == null)
      return "Not specified";

    var duration = BookingRequest.EndDate - BookingRequest.StartDate;
    var totalMinutes = (int)duration.TotalMinutes;

    if (totalMinutes < 60)
    {
      return $"{totalMinutes} minutes";
    }
    else
    {
      var hours = totalMinutes / 60;
      var minutes = totalMinutes % 60;

      if (minutes == 0)
      {
        return $"{hours} hour{(hours > 1 ? "s" : "")}";
      }
      else
      {
        return $"{hours} hour{(hours > 1 ? "s" : "")} {minutes} minute{(minutes > 1 ? "s" : "")}";
      }
    }
  }

  private bool IsValidBooking()
  {
    return BookingRequest != null &&
           SelectedPetWalker != null &&
           SelectedPet != null &&
           BookingRequest.PetWalkerId != Guid.Empty &&
           BookingRequest.PetId != Guid.Empty &&
           BookingRequest.PetOwnerId != Guid.Empty &&
           BookingRequest.StartDate < BookingRequest.EndDate;
  }

  protected override void OnParametersSet()
  {
    if (!IsValidBooking())
    {
      Logger.LogWarning("Invalid booking data provided to confirmation component");
    }
  }
}
