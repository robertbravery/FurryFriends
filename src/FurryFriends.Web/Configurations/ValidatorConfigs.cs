using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.Web.Endpoints.ClientEndpoints.AddClientPet;
using FurryFriends.Web.Endpoints.ClientEndpoints.Create;
using FurryFriends.Web.Endpoints.ClientEndpoints.Get;
using FurryFriends.Web.Endpoints.ClientEndpoints.RemovePet;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;
using FurryFriends.Web.Endpoints.RatingEndpoints.Create;
using FurryFriends.Web.Endpoints.RatingEndpoints.Update;
using FurryFriends.Web.Endpoints.RatingEndpoints.Delete;

namespace FurryFriends.Web.Configurations;

public static class ValidatorConfigs
{
  public static IServiceCollection AddValidatorConfigs(this IServiceCollection services)
  {

    services.AddScoped<IValidator<CreatePetWalkerRequest>, CreatePetWalkerRequestValidator>();
    services.AddScoped<IValidator<CreateClientRequest>, CreateClientRequestValidator>();
    services.AddScoped<IValidator<GetClientRequest>, GetClientRequestValidator>();
    services.AddScoped<IValidator<AddPetRequest>, AddPetRequestValidatior>();
    services.AddScoped<IValidator<RemovePetRequest>, RemovePetRequestValidator>();

    // Rating endpoint validators
    services.AddScoped<IValidator<CreateRatingRequest>, CreateRatingValidator>();
    services.AddScoped<IValidator<UpdateRatingRequest>, UpdateRatingValidator>();
    services.AddScoped<IValidator<DeleteRatingRequest>, DeleteRatingValidator>();

    //ToDO: Remove Core Validators
    services.AddTransient<IValidator<Name>, NameValidator>();
    services.AddSingleton<IValidator<PhoneNumber>, PhoneNumberValidator>();
    services.AddSingleton<IValidator<Compensation>, CompensationValidator>();


    return services;
  }
}
