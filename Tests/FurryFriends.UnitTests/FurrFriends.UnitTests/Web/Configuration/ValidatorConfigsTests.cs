using FurryFriends.Core.ValueObjects;
using FurryFriends.Web.Configurations;
using FurryFriends.Web.Endpoints.BookingEndpoints.Cancel;
using FurryFriends.Web.Endpoints.ClientEndpoints.AddClientPet;
using FurryFriends.Web.Endpoints.ClientEndpoints.Create;
using FurryFriends.Web.Endpoints.ClientEndpoints.Delete;
using FurryFriends.Web.Endpoints.ClientEndpoints.Get;
using FurryFriends.Web.Endpoints.ClientEndpoints.RemovePet;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;
using FurryFriends.Web.Endpoints.RatingEndpoints.Create;
using FurryFriends.Web.Endpoints.RatingEndpoints.Delete;
using FurryFriends.Web.Endpoints.RatingEndpoints.GetPetWalkerRatingSummary;
using FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;
using FurryFriends.Web.Endpoints.RatingEndpoints.Update;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.Booking;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FurryFriends.UnitTests.Web.Configuration;

public class ValidatorConfigsTests
{
    private readonly IServiceCollection _services;

    public ValidatorConfigsTests()
    {
        _services = new ServiceCollection();
        _services.AddValidatorConfigs();
    }

    [Fact]
    public void AddValidatorConfigs_RegistersAllClientValidators()
    {
        // Assert - Client validators
        var createClientValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<CreateClientRequest>));
        createClientValidator.Should().NotBeNull();
        createClientValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var getClientValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<GetClientRequest>));
        getClientValidator.Should().NotBeNull();
        getClientValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var addPetValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<AddPetRequest>));
        addPetValidator.Should().NotBeNull();
        addPetValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var removePetValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<RemovePetRequest>));
        removePetValidator.Should().NotBeNull();
        removePetValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var deleteClientValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<DeleteClientRequest>));
        deleteClientValidator.Should().NotBeNull();
        deleteClientValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddValidatorConfigs_RegistersPetWalkerValidators()
    {
        var createPetWalkerValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<CreatePetWalkerRequest>));
        createPetWalkerValidator.Should().NotBeNull();
        createPetWalkerValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddValidatorConfigs_RegistersAllRatingValidators()
    {
        var createRatingValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<CreateRatingRequest>));
        createRatingValidator.Should().NotBeNull();
        createRatingValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var updateRatingValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<UpdateRatingRequest>));
        updateRatingValidator.Should().NotBeNull();
        updateRatingValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var deleteRatingValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<DeleteRatingRequest>));
        deleteRatingValidator.Should().NotBeNull();
        deleteRatingValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var summaryValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<GetPetWalkerRatingSummaryRequest>));
        summaryValidator.Should().NotBeNull();
        summaryValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var listRatingsValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<GetRatingsForPetWalkerRequest>));
        listRatingsValidator.Should().NotBeNull();
        listRatingsValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddValidatorConfigs_RegistersAllTimeslotValidators()
    {
        var createTimeslotValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<CreateTimeslotRequest>));
        createTimeslotValidator.Should().NotBeNull();
        createTimeslotValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var updateTimeslotValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<UpdateTimeslotRequest>));
        updateTimeslotValidator.Should().NotBeNull();
        updateTimeslotValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var requestCustomTimeValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<RequestCustomTimeRequest>));
        requestCustomTimeValidator.Should().NotBeNull();
        requestCustomTimeValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var respondCustomTimeValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<RespondToCustomTimeRequestRequest>));
        respondCustomTimeValidator.Should().NotBeNull();
        respondCustomTimeValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var bookTimeslotValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<BookTimeslotRequest>));
        bookTimeslotValidator.Should().NotBeNull();
        bookTimeslotValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddValidatorConfigs_RegistersBookingValidators()
    {
        var cancelBookingValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<CancelBookingRequest>));
        cancelBookingValidator.Should().NotBeNull();
        cancelBookingValidator!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddValidatorConfigs_RegistersCoreValueValidators()
    {
        var nameValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<Name>));
        nameValidator.Should().NotBeNull();
        nameValidator!.Lifetime.Should().Be(ServiceLifetime.Transient);

        var phoneValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<PhoneNumber>));
        phoneValidator.Should().NotBeNull();
        phoneValidator!.Lifetime.Should().Be(ServiceLifetime.Singleton);

        var compensationValidator = _services.FirstOrDefault(s =>
            s.ServiceType == typeof(IValidator<Compensation>));
        compensationValidator.Should().NotBeNull();
        compensationValidator!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddValidatorConfigs_RegistersAllValidators_CountCheck()
    {
        // Total registrations: 5 client + 1 petwalker + 5 rating + 5 timeslot + 1 booking + 3 core = 20
        var validatorRegistrations = _services
            .Where(s => s.ServiceType.IsGenericType &&
                        s.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .ToList();

        validatorRegistrations.Should().HaveCount(20);
    }

    [Fact]
    public void AddValidatorConfigs_NoDuplicateRegistrations()
    {
        // Verify no duplicate service registrations for the same type
        var validatorRegistrations = _services
            .Where(s => s.ServiceType.IsGenericType &&
                        s.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .GroupBy(s => s.ServiceType)
            .ToList();

        // Each validator type should be registered exactly once
        foreach (var group in validatorRegistrations)
        {
            group.Count().Should().Be(1, $"Validator {group.Key.Name} is registered more than once");
        }
    }

    [Fact]
    public void AddValidatorConfigs_AllValidatorTypesResolveSuccessfully()
    {
        // Build the service provider and verify all validators can be resolved
        var serviceProvider = _services.BuildServiceProvider();

        // Client validators
        var createClientValidator = serviceProvider.GetService<IValidator<CreateClientRequest>>();
        createClientValidator.Should().NotBeNull();
        createClientValidator.Should().BeOfType<CreateClientRequestValidator>();

        var deleteClientValidator = serviceProvider.GetService<IValidator<DeleteClientRequest>>();
        deleteClientValidator.Should().NotBeNull();
        deleteClientValidator.Should().BeOfType<DeleteClientRequestValidator>();

        var addPetValidator = serviceProvider.GetService<IValidator<AddPetRequest>>();
        addPetValidator.Should().NotBeNull();
        addPetValidator.Should().BeOfType<AddPetRequestValidator>();

        // Rating validators
        var createRatingValidator = serviceProvider.GetService<IValidator<CreateRatingRequest>>();
        createRatingValidator.Should().NotBeNull();
        createRatingValidator.Should().BeOfType<CreateRatingValidator>();

        var updateRatingValidator = serviceProvider.GetService<IValidator<UpdateRatingRequest>>();
        updateRatingValidator.Should().NotBeNull();
        updateRatingValidator.Should().BeOfType<UpdateRatingValidator>();

        var deleteRatingValidator = serviceProvider.GetService<IValidator<DeleteRatingRequest>>();
        deleteRatingValidator.Should().NotBeNull();
        deleteRatingValidator.Should().BeOfType<DeleteRatingValidator>();

        // Timeslot validators
        var createTimeslotValidator = serviceProvider.GetService<IValidator<CreateTimeslotRequest>>();
        createTimeslotValidator.Should().NotBeNull();
        createTimeslotValidator.Should().BeOfType<CreateTimeslotValidator>();

        var bookTimeslotValidator = serviceProvider.GetService<IValidator<BookTimeslotRequest>>();
        bookTimeslotValidator.Should().NotBeNull();
        bookTimeslotValidator.Should().BeOfType<BookTimeslotValidator>();

        // Booking validators
        var cancelBookingValidator = serviceProvider.GetService<IValidator<CancelBookingRequest>>();
        cancelBookingValidator.Should().NotBeNull();
        cancelBookingValidator.Should().BeOfType<CancelBookingValidator>();
    }
}
