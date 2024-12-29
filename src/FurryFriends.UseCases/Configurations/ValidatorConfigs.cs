using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users.Create;
using Microsoft.Extensions.DependencyInjection;

namespace FurryFriends.UseCases.Configurations;

public static class ValidatorConfigs
{
    public static IServiceCollection AddUseCaseValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<PhoneNumberValidator>();
        services.AddValidatorsFromAssemblyContaining<NameValidator>();

        return services;
    }
}