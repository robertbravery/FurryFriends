using FluentValidation;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

public class CreatePetWalkerRequestValidator : AbstractValidator<CreatePetWalkerRequest>
{
  public CreatePetWalkerRequestValidator()
  {
    RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required.");
    RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required.");
    RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
    RuleFor(x => x.CountryCode).NotEmpty().WithMessage("Country code is required.");
    RuleFor(x => x.Number).NotEmpty().WithMessage("Number is required.");
    RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.");
    RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
    RuleFor(x => x.State).NotEmpty().WithMessage("State is required.");
    RuleFor(x => x.PostalCode).NotEmpty().WithMessage("Postal code is required.");
    RuleFor(x => x.Gender).NotNull().WithMessage("Gender is required.");
    RuleFor(x => x.DateOfBirth).NotNull().WithMessage("Date of birth is required.");
    RuleFor(x => x.HourlyRate).NotEmpty().WithMessage("Hourly rate is required.");
    RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is required.");
    RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0).WithMessage("Years of experience must be greater than or equal to 0.");
    RuleFor(x => x.DailyPetWalkLimit).GreaterThanOrEqualTo(0).WithMessage("Daily pet walk limit must be greater than or equal to 0.");
  }
}
