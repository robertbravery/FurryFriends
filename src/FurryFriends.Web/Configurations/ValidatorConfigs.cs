using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users.CreateUser;

namespace FurryFriends.Web.Configurations;

public static class ValidatorConfigs
{
  public static IServiceCollection AddValidatorConfigs(this IServiceCollection services)
  {
    services.AddSingleton<IValidator<Name>, NameValidator>();
    services.AddSingleton<IValidator<PhoneNumber>, PhoneNumberValidator>();
    services.AddSingleton<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
    services.AddSingleton<IValidator<Compensation>, CompensationValidator>();

    return services;
  }
}
