namespace FurryFriends.Web.Endpoints.RatingEndpoints.Delete;

public class DeleteRatingValidator : Validator<DeleteRatingRequest>
{
    public DeleteRatingValidator()
    {
        RuleFor(x => x.RatingId)
            .NotEmpty()
            .WithMessage("RatingId is required");
    }
}
