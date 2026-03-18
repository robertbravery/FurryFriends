using FluentValidation;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class CreateTimeslotValidator : AbstractValidator<CreateTimeslotRequest>
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
            .InclusiveBetween(15, 120)
            .WithMessage("DurationInMinutes must be between 15 and 120 minutes.");
    }
}
