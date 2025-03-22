using FluentValidation;

namespace FurryFriends.UseCases.Domain.Clients.Command.RemovePet;

public class RemovePetCommandValidator : AbstractValidator<RemovePetCommand>
{
    public RemovePetCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        RuleFor(x => x.PetId)
            .NotEmpty()
            .WithMessage("Pet ID is required");
    }
}