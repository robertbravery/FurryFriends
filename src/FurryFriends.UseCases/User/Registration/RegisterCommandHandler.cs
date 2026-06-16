using Ardalis.Result;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using FurryFriends.Core.Interfaces;

namespace FurryFriends.UseCases.User.Registration;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly IEmailSender _emailSender;

    public RegisterCommandHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(request.Email, nameof(request.Email));
        Guard.Against.NullOrWhiteSpace(request.Password, nameof(request.Password));
        Guard.Against.NullOrWhiteSpace(request.Role, nameof(request.Role));

        var user = new Core.UserAggregate.User(request.Email, request.Password, request.Role)
        {
            Id = Guid.NewGuid()
        };

        await _emailSender.SendConfirmationEmail(request.Email);

        return Result.Success(user.Id);
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(c => c.Password)
            .NotEmpty()
            .Length(8, 100)
            .Must(HasComplexity)
            .WithMessage("Password must contain uppercase, lowercase, numeric, and special characters");

        RuleFor(c => c.Role)
            .Must(IsValidRole)
            .WithMessage("Invalid role. Valid roles: Admin, Petwalker, Client");
    }

    private bool HasComplexity(string password)
    {
        return password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(c => !char.IsLetterOrDigit(c));
    }

    private bool IsValidRole(string role)
    {
        return new[] { "Admin", "Petwalker", "Client" }.Contains(role);
    }
}