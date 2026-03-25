using FluentValidation;

namespace FurryFriends.UseCases.Rating.UpdateRating;

public class UpdateRatingValidator : AbstractValidator<UpdateRatingCommand>
{
    public UpdateRatingValidator()
    {
        RuleFor(x => x.RatingId)
            .NotEmpty()
            .WithMessage("Rating ID is required");

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(1, 5)
            .When(x => x.RatingValue.HasValue)
            .WithMessage("Rating value must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters");

        RuleFor(x => x)
            .Must(x => x.RatingValue.HasValue || !string.IsNullOrEmpty(x.Comment))
            .WithMessage("At least one of RatingValue or Comment must be provided");
    }
}
