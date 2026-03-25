using FluentValidation;

namespace FurryFriends.UseCases.Rating.CreateRating;

public class CreateRatingValidator : AbstractValidator<CreateRatingCommand>
{
    public CreateRatingValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required");

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating value must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters");
    }
}
