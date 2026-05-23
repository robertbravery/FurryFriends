using FluentValidation;

namespace FurryFriends.UseCases.Domain.Ratings.GetRatingsForPetWalker;

public class GetRatingsForPetWalkerValidator : AbstractValidator<GetRatingsForPetWalkerQuery>
{
    public GetRatingsForPetWalkerValidator()
    {
        RuleFor(x => x.PetWalkerId)
            .NotEmpty()
            .WithMessage("Pet Walker ID is required");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}
