using FluentValidation;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

public class RequestCustomTimeValidator : AbstractValidator<RequestCustomTimeRequest>
{
    public RequestCustomTimeValidator()
    {
        RuleFor(x => x.PetWalkerId)
            .NotEmpty()
            .WithMessage("Petwalker ID is required.");

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required.");

        RuleFor(x => x.RequestedDate)
            .NotEmpty()
            .WithMessage("Requested date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Requested date must be today or in the future.");

        RuleFor(x => x.PreferredStartTime)
            .NotEmpty()
            .WithMessage("Preferred start time is required.");

        RuleFor(x => x.PreferredDurationMinutes)
            .InclusiveBetween(30, 45)
            .WithMessage("Duration must be between 30 and 45 minutes.");

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
