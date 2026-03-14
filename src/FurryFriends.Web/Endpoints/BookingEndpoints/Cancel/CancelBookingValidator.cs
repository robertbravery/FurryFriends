using FluentValidation;
using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Cancel;

public class CancelBookingValidator : Validator<CancelBookingRequest>
{
  public CancelBookingValidator()
  {
    RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("Booking ID is required");

    RuleFor(x => x.Reason)
        .IsInEnum()
        .WithMessage("Reason must be a valid cancellation reason");

    RuleFor(x => x.CancelledBy)
        .IsInEnum()
        .WithMessage("CancelledBy must be either Client or PetWalker");

    RuleFor(x => x.AdditionalNotes)
        .MaximumLength(500)
        .When(x => x.AdditionalNotes != null)
        .WithMessage("Additional notes must not exceed 500 characters");
  }
}