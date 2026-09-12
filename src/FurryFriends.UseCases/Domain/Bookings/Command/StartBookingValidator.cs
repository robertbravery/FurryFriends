using FluentValidation;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class StartBookingValidator : AbstractValidator<StartBookingCommand>
{
  public StartBookingValidator()
  {
    RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("Booking ID is required");
  }
}
