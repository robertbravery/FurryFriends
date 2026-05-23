namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetPetWalkerRatingSummary;

public class GetPetWalkerRatingSummaryValidator : Validator<GetPetWalkerRatingSummaryRequest>
{
    public GetPetWalkerRatingSummaryValidator()
    {
        RuleFor(x => x.PetWalkerId)
            .NotEmpty()
            .WithMessage("PetWalkerId is required");
    }
}
