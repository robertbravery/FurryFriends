using FluentValidation;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

public class RespondToCustomTimeRequestValidator : AbstractValidator<RespondToCustomTimeRequestRequest>
{
    private static readonly string[] ValidResponses = { "Accept", "Decline", "CounterOffer" };

    public RespondToCustomTimeRequestValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("Request ID is required.");

        RuleFor(x => x.Response)
            .NotEmpty()
            .WithMessage("Response is required.")
            .Must(r => ValidResponses.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Response must be Accept, Decline, or CounterOffer.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");

        When(x => x.Response.Equals("Decline", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason is required when declining a request.");
        });

        When(x => x.Response.Equals("CounterOffer", StringComparison.OrdinalIgnoreCase), () =>
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
