using FurryFriends.UseCases.Domain.Clients.Command.AddPet;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.AddClientPet;

public class AddPetRequestValidatior : AbstractValidator<AddPetCommand>
{

  public AddPetRequestValidatior()
  {
    RuleFor(x => x.ClientId)
        .NotEmpty().WithMessage("Client ID is required");

    // Core properties validation
    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Pet name is required")
        .MaximumLength(50).WithMessage("Pet name cannot exceed 50 characters");

    RuleFor(x => x.BreedId)
        .GreaterThan(0).WithMessage("Valid breed ID is required");

    RuleFor(x => x.Age)
        .GreaterThan(0).WithMessage("Age must be greater than 0");


    RuleFor(x => x.Weight)
        .GreaterThan(0.1).WithMessage("Weight must be greater than 0.1")
        .LessThan(200).WithMessage("Weight must be less than 200");

    RuleFor(x => x.Color)
        .NotEmpty().WithMessage("Color is required")
        .MaximumLength(30).WithMessage("Color cannot exceed 30 characters");

    // Optional properties validation
    RuleFor(x => x.MedicalConditions)
        .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.MedicalConditions))
        .WithMessage("Medical conditions cannot exceed 500 characters");

    RuleFor(x => x.DietaryRestrictions)
        .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.DietaryRestrictions))
        .WithMessage("Dietary restrictions cannot exceed 500 characters");

    RuleFor(x => x.FavoriteActivities)
        .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.FavoriteActivities))
        .WithMessage("Favorite activities cannot exceed 500 characters");

    RuleFor(x => x.SpecialNeeds)
        .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.SpecialNeeds))
        .WithMessage("Special needs cannot exceed 500 characters");

  }
}
