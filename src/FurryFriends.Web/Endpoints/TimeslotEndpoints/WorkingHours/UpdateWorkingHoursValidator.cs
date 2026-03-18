using FluentValidation;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class UpdateWorkingHoursValidator : Validator<UpdateWorkingHoursRequest>
{
    public UpdateWorkingHoursValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Working hours ID is required");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("End time must be after start time")
            .When(x => x.StartTime != default && x.EndTime != default);
    }
}
