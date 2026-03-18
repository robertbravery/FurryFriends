using FluentValidation;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

public class DeleteTimeslotValidator : AbstractValidator<DeleteTimeslotCommand>
{
    public DeleteTimeslotValidator()
    {
        RuleFor(x => x.TimeslotId)
            .NotEmpty()
            .WithMessage("TimeslotId is required.");
    }
}
