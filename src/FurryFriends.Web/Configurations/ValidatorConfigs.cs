using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users.CreateUser;
using FurryFriends.Web.Endpoints.ClientEnpoints.Create;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

namespace FurryFriends.Web.Configurations;

public static class ValidatorConfigs
{
  public static IServiceCollection AddValidatorConfigs(this IServiceCollection services)
  {
    services.AddSingleton<IValidator<CreatePetWalkerRequest>, CreatePetWalkerRequestValidator>();
    services.AddSingleton<IValidator<CreateClientRequest>, CreateClientRequestValidator>();

    services.AddValidatorsFromAssemblyContaining<CreatePetWalkerCommandValidator>();
    services.AddSingleton<IValidator<Name>, NameValidator>();
    services.AddSingleton<IValidator<PhoneNumber>, PhoneNumberValidator>();
    services.AddSingleton<IValidator<CreatePetWalkerCommand>, CreatePetWalkerCommandValidator>();
    services.AddSingleton<IValidator<Compensation>, CompensationValidator>();


    return services;
  }
}
