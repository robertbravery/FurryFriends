using FluentValidation;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Create;

public class CreateRatingValidator : Validator<CreateRatingRequest>
{
    public CreateRatingValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("BookingId is required");

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(1, 5)
            .WithMessage("RatingValue must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}
