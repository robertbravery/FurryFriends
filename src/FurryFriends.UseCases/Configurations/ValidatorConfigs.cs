using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Users.Create;
using Microsoft.Extensions.DependencyInjection;

namespace FurryFriends.UseCases.Configurations;

public static class ValidatorConfigs
{
    public static IServiceCollection AddUseCasesValidators(this IServiceCollection services)
    {
        // Register validators used in use cases
        services.AddScoped<IValidator<Name>, NameValidator>();
        services.AddScoped<IValidator<PhoneNumber>, PhoneNumberValidator>();
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();

        return services;
    }
}