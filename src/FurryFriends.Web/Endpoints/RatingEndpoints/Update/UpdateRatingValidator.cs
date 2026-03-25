using FluentValidation;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Update;

public class UpdateRatingValidator : Validator<UpdateRatingRequest>
{
    public UpdateRatingValidator()
    {
        RuleFor(x => x.RatingId)
            .NotEmpty()
            .WithMessage("RatingId is required");

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(1, 5)
            .WithMessage("RatingValue must be between 1 and 5")
            .When(x => x.RatingValue.HasValue);

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));

        RuleFor(x => x)
            .Must(x => x.RatingValue.HasValue || !string.IsNullOrEmpty(x.Comment))
            .WithMessage("At least one of RatingValue or Comment must be provided");
    }
}
