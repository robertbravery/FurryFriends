using FluentValidation;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalkerByLocationValidator : Validator<ListPetWalkerByLocationRequest>
{
  public ListPetWalkerByLocationValidator()
  {
    RuleFor(x => x.Page)
        .GreaterThan(0)
        .WithMessage("Page number must be greater than 0");

    RuleFor(x => x.PageSize)
        .GreaterThan(0)
        .WithMessage("Page size must be greater than 0")
        .LessThanOrEqualTo(100)
        .WithMessage("Page size cannot exceed 100 items");
  }
}

