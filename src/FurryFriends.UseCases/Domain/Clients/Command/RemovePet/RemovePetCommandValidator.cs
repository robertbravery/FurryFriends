using FluentValidation;

namespace FurryFriends.UseCases.Domain.Clients.Command.RemovePet;

public class RemovePetCommandValidator : AbstractValidator<RemovePetCommand>
{
    public RemovePetCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Request cannot be empty");

        RuleFor(x => x.PetId)
            .NotEmpty()
            .WithMessage("Pet ID is required");
    
    }
}