using FluentValidation;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

public class CreateTimeslotValidator : AbstractValidator<CreateTimeslotCommand>
{
    public CreateTimeslotValidator()
    {
        RuleFor(x => x.PetWalkerId)
            .NotEmpty()
            .WithMessage("PetWalkerId is required.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date cannot be in the past.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("StartTime is required.");

        RuleFor(x => x.DurationInMinutes)
            .InclusiveBetween(30, 45)
            .WithMessage("DurationInMinutes must be between 30 and 45 minutes.");
    }
}
