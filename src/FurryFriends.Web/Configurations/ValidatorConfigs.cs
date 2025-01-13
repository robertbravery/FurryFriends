using FluentValidation;
using FurryFriends.Web.Endpoints.ClientEnpoints.Create;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

namespace FurryFriends.Web.Configurations;

public static class ValidatorConfigs
{
  public static IServiceCollection AddValidatorConfigs(this IServiceCollection services)
  {
    services.AddScoped<IValidator<CreatePetWalkerRequest>, CreatePetWalkerRequestValidator>();
    services.AddScoped<IValidator<CreateClientRequest>, CreateClientRequestValidator>();
    //services.AddSingleton<IValidator<Name>, NameValidator>();
    //services.AddSingleton<IValidator<PhoneNumber>, PhoneNumberValidator>();
    //services.AddSingleton<IValidator<CreatePetWalkerCommand>, CreatePetWalkerCommandValidator>();
    //services.AddSingleton<IValidator<Compensation>, CompensationValidator>();


    return services;
  }
}
