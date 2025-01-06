using FluentValidation;
using FurryFriends.Core.ContributorAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users.CreateUser;
using FurryFriends.UseCases.Users.UpdateUser;
using Microsoft.Extensions.DependencyInjection;

namespace FurryFriends.UseCases.Configurations;

public static class ValidatorConfigs
{
  public static IServiceCollection AddUseCaseValidators(this IServiceCollection services)
  {
    services.AddValidatorsFromAssemblyContaining<CreatePetWalkerCommandValidator>();
    services.AddValidatorsFromAssemblyContaining<PhoneNumberValidator>();
    services.AddValidatorsFromAssemblyContaining<NameValidator>();
    services.AddTransient<IValidator<Address>, AddressValidator>();
    services.AddTransient<IValidator<Compensation>, CompensationValidator>();
    services.AddTransient<IValidator<UpdatePetWalkerHourlyRateCommand>, UpdatePetWalkerHourlyRateCommandValidator>();


    return services;
  }
}
