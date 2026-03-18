using FluentValidation;

namespace FurryFriends.UseCases.Timeslots.Booking;

public class BookTimeslotValidator : AbstractValidator<BookTimeslotCommand>
{
    public BookTimeslotValidator()
    {
        RuleFor(x => x.TimeslotId)
            .NotEmpty()
            .WithMessage("Timeslot ID is required.");

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required.");

        RuleFor(x => x.ClientAddress)
            .NotEmpty()
            .WithMessage("Service address is required.")
            .MaximumLength(500)
            .WithMessage("Service address cannot exceed 500 characters.");

        RuleFor(x => x.PetIds)
            .NotEmpty()
            .WithMessage("At least one pet is required.")
            .Must(petIds => petIds.Count <= 5)
            .WithMessage("Maximum 5 pets per booking.");
    }
}
