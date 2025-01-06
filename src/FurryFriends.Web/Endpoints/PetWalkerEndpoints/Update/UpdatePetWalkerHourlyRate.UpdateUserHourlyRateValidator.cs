using FluentValidation;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Update;

public class UpdateUserHourlyRateValidator : Validator<UpdateUserHourlyRateRequest>
{
  public UpdateUserHourlyRateValidator()
  {
    RuleFor(x => x.UserId)
        .NotEmpty()
        .WithMessage("User ID is required");

    RuleFor(x => x.HourlyRate)
        .GreaterThan(0)
        .WithMessage("Hourly rate must be greater than 0");

    RuleFor(x => x.Currency)
        .NotEmpty()
        .WithMessage("Currency is required")
        .Length(3)
        .WithMessage("Currency must be a 3-letter code")
        .Matches("^[A-Z]{3}$")
        .WithMessage("Currency must be in uppercase ISO format (e.g., USD, EUR)");
  }
}
