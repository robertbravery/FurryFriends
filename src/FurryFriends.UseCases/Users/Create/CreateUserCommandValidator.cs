using System;
using FluentValidation;

namespace FurryFriends.UseCases.Users.Create;

public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
public CreateUserCommandValidator()
        {
            RuleFor(cmd => cmd.FirstName)
                .NotEmpty().WithMessage("First Name cannot be empty")
                .MaximumLength(30)
                .MinimumLength(5).WithMessage("Name must be 5-30 characters long");
            RuleFor(cmd => cmd.LastName)
                .NotEmpty().WithMessage("Last Name cannot be empty")
                .MaximumLength(30)
                .MinimumLength(5).WithMessage("Last Name must be 5-30 characters long");
            RuleFor(cmd => cmd.Email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .EmailAddress().WithMessage("Invalid email address");
            RuleFor(cmd => cmd.CountryCode)
                .NotEmpty().WithMessage("Country code cannot be empty");
            RuleFor(cmd => cmd.AreaCode)
                .NotEmpty().WithMessage("Area code cannot be empty");
            RuleFor(cmd => cmd.Number)
                .NotEmpty().WithMessage("Phone number cannot be empty");
            RuleFor(cmd => cmd.Street).NotEmpty().WithMessage("Street cannot be empty");
            RuleFor(cmd => cmd.City).NotEmpty().WithMessage("City cannot be empty");
            RuleFor(cmd => cmd.State).NotEmpty().WithMessage("State cannot be empty");
            RuleFor(cmd => cmd.ZipCode)
                .NotEmpty().WithMessage("Zip code cannot be empty")
                .MaximumLength(5)
                .MinimumLength(4).WithMessage("Zip code must be 4-5 digits long");
        }
}
