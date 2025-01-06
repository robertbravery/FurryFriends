using FluentValidation;

namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUserByLocationValidator : Validator<ListUsersByLocationRequest>
{
  public ListUserByLocationValidator()
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

