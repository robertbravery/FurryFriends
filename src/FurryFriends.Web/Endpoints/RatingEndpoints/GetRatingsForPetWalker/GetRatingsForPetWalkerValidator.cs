namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

public class GetRatingsForPetWalkerValidator : Validator<GetRatingsForPetWalkerRequest>
{
    public GetRatingsForPetWalkerValidator()
    {
        RuleFor(x => x.PetWalkerId)
            .NotEmpty()
            .WithMessage("PetWalkerId is required");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}
