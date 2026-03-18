using FluentValidation;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

public class CreateWorkingHoursValidator : AbstractValidator<CreateWorkingHoursCommand>
{
    public CreateWorkingHoursValidator()
    {
        RuleFor(x => x.PetWalkerId)
            .NotEmpty()
            .WithMessage("PetWalkerId is required")
            .WithErrorCode("InvalidPetWalkerId");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum()
            .WithMessage("Day of week is required")
            .WithErrorCode("InvalidDayOfWeek");

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
