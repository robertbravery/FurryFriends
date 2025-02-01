using FluentValidation;

namespace FurryFriends.UseCase.Users.CreatePetWalker;

public class CreatePetWalkerCommandValidator : AbstractValidator<CreatePetWalkerCommand>
{
  public CreatePetWalkerCommandValidator()
  {
    RuleFor(r => r.DateOfBirth).NotEmpty().WithMessage("Date of Birth is required.")
       .LessThan(DateTime.Now).WithMessage("Date of Birth must be in the past.")
       .GreaterThan(DateTime.Now.AddYears(-16)).WithMessage("PetWalker must be at least 16 years old.");

    RuleFor(r => r.YearsOfExperience).NotEmpty().WithMessage("Years of experience is required.")
      .GreaterThan(0).WithMessage("Years of experience must be greater than 0.");

    RuleFor(r => r.DailyPetWalkLimit).NotEmpty().WithMessage("Daily pet walk limit is required.");

    RuleFor(r => r.HourlyRate).NotEmpty().WithMessage("Hourly rate is required.");
  }
}
