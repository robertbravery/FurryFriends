using FluentValidation;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.ContributorAggregate;

public class NameValidator : AbstractValidator<Name>
{
  public NameValidator()
  {
    RuleFor(n => n.FirstName)
        .NotEmpty().WithMessage("First name cannot be null or whitespace.")
        .MinimumLength(2).WithMessage("First name must be at least 2 characters long.")
        .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.")
        .Matches("^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$")
        .WithMessage("First name can only contain letters, spaces, and characters: ' , . -");

    RuleFor(n => n.LastName)
        .NotEmpty().WithMessage("Last name cannot be null or whitespace.")
        .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.")
        .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.")
        .Matches("^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$")
        .WithMessage("Last name can only contain letters, spaces, and characters: ' , . -");
        
  }
}
