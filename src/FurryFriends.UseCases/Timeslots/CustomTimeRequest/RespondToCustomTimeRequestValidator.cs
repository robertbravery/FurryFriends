using FluentValidation;

namespace FurryFriends.UseCases.Timeslots.CustomTimeRequest;

public class RespondToCustomTimeRequestValidator : AbstractValidator<RespondToCustomTimeRequestCommand>
{
    public RespondToCustomTimeRequestValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("Request ID is required.");

        RuleFor(x => x.Response)
            .IsInEnum()
            .WithMessage("Response must be Accept, Decline, or CounterOffer.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");

        When(x => x.Response == CustomTimeRequestResponse.Decline, () =>
        {
            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason is required when declining a request.");
        });

        When(x => x.Response == CustomTimeRequestResponse.CounterOffer, () =>
        {
            RuleFor(x => x.CounterOfferedDate)
                .NotEmpty()
                .WithMessage("Counter-offered date is required when counter-offering.");

            RuleFor(x => x.CounterOfferedTime)
                .NotEmpty()
                .WithMessage("Counter-offered time is required when counter-offering.");
                
            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason is required when counter-offering.");
        });
    }
}
