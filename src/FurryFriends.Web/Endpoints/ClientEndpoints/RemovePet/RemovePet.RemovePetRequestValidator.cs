namespace FurryFriends.Web.Endpoints.ClientEndpoints.RemovePet;

public class RemovePetRequestValidator : Validator<RemovePetRequest>
{
    public RemovePetRequestValidator()
    {
       RuleFor(x => x).NotEmpty().WithMessage("Request cannot be empty");
        RuleFor(x => x.PetId)
            .NotEmpty()
            .WithMessage("Pet ID is required");
    }
}