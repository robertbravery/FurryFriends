using FluentValidation;

namespace FurryFriends.UseCases.Domain.Ratings.DeleteRating;

public class DeleteRatingValidator : AbstractValidator<DeleteRatingCommand>
{
    public DeleteRatingValidator()
    {
        RuleFor(x => x.RatingId)
            .NotEmpty()
            .WithMessage("Rating ID is required");

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");
    }
}
