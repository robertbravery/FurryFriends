using FluentValidation;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

public class UpdateTimeslotValidator : AbstractValidator<UpdateTimeslotCommand>
{
    public UpdateTimeslotValidator()
    {
        RuleFor(x => x.TimeslotId)
            .NotEmpty()
            .WithMessage("TimeslotId is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("StartTime is required.");

        RuleFor(x => x.DurationInMinutes)
            .InclusiveBetween(30, 45)
            .WithMessage("DurationInMinutes must be between 30 and 45 minutes.");
    }
}
