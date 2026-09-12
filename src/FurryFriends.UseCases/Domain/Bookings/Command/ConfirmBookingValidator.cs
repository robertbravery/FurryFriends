using FluentValidation;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class ConfirmBookingValidator : AbstractValidator<ConfirmBookingCommand>
{
  public ConfirmBookingValidator()
  {
    RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("Booking ID is required");
  }
}
