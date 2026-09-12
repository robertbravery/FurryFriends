using FluentValidation;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class CompleteBookingValidator : AbstractValidator<CompleteBookingCommand>
{
  public CompleteBookingValidator()
  {
    RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("Booking ID is required");
  }
}
