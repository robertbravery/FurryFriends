using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Users.Create;
using FurryFriends.Web.Endpoints;
using FurryFriends.Web.Endpoints.UserEndpoints.Create;

namespace FurryFriends.Web.Configurations;

public static class ValidatorConfigs
{
    public static IServiceCollection AddValidatorConfigs(this IServiceCollection services)
    {
         // Register validators from Core project
        services.AddSingleton<IValidator<Name>, NameValidator>();
        services.AddSingleton<IValidator<PhoneNumber>, PhoneNumberValidator>();

        // Register validators from UseCases project if needed
        services.AddSingleton<IValidator<CreateUserCommand>, CreateUserCommandValidator>();

       // Register validators for UserEndpoints.Create endpoint
        services.AddSingleton<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        // services.AddScoped< CreateUserRequestValidator>();
        return services;
    }
}