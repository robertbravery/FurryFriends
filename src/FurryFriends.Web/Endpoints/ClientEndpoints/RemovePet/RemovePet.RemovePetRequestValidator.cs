namespace FurryFriends.Web.Endpoints.ClientEndpoints.RemovePet;

public class RemovePetRequestValidator : Validator<RemovePetRequest>
{
    public RemovePetRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        RuleFor(x => x.PetId)
            .NotEmpty()
            .WithMessage("Pet ID is required");
    }
}