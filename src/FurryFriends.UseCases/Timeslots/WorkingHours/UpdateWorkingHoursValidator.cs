using FluentValidation;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

public class UpdateWorkingHoursValidator : AbstractValidator<UpdateWorkingHoursCommand>
{
    public UpdateWorkingHoursValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Working hours ID is required")
            .WithErrorCode("InvalidWorkingHoursId");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required")
            .WithErrorCode("InvalidStartTime");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required")
            .WithErrorCode("InvalidEndTime");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("End time must be after start time")
            .WithErrorCode("InvalidTimeRange")
            .When(x => x.StartTime != default && x.EndTime != default);
    }
}
