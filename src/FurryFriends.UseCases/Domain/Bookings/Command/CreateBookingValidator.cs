// Application/Scheduling/Commands/CreateBookingValidator.cs
using FluentValidation;

public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
  public CreateBookingValidator()
  {
    RuleFor(x => x.Start)
        .LessThan(x => x.End)
        .WithMessage("Start must be before End");

    RuleFor(x => x.PetWalkerId).NotEmpty();
    RuleFor(x => x.PetOwnerId).NotEmpty();
  }
}
