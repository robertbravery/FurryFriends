using FluentValidation;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class MarkBookingNoShowValidator : AbstractValidator<MarkBookingNoShowCommand>
{
  public MarkBookingNoShowValidator()
  {
    RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("Booking ID is required");
  }
}
