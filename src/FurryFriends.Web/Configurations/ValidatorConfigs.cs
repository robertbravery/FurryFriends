using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
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

namespace FurryFriends.Web.Configurations;

public static class ValidatorConfigs
{
  public static IServiceCollection AddValidatorConfigs(this IServiceCollection services)
  {
    // Client validators
    services.AddScoped<IValidator<CreateClientRequest>, CreateClientRequestValidator>();
    services.AddScoped<IValidator<GetClientRequest>, GetClientRequestValidator>();
    services.AddScoped<IValidator<AddPetRequest>, AddPetRequestValidator>();
    services.AddScoped<IValidator<RemovePetRequest>, RemovePetRequestValidator>();
    services.AddScoped<IValidator<DeleteClientRequest>, DeleteClientRequestValidator>();

    // PetWalker validators
    services.AddScoped<IValidator<CreatePetWalkerRequest>, CreatePetWalkerRequestValidator>();

    // Rating endpoint validators
    services.AddScoped<IValidator<CreateRatingRequest>, CreateRatingValidator>();
    services.AddScoped<IValidator<UpdateRatingRequest>, UpdateRatingValidator>();
    services.AddScoped<IValidator<DeleteRatingRequest>, DeleteRatingValidator>();
    services.AddScoped<IValidator<GetPetWalkerRatingSummaryRequest>, GetPetWalkerRatingSummaryValidator>();
    services.AddScoped<IValidator<GetRatingsForPetWalkerRequest>, GetRatingsForPetWalkerValidator>();

    // Timeslot validators
    services.AddScoped<IValidator<CreateTimeslotRequest>, CreateTimeslotValidator>();
    services.AddScoped<IValidator<UpdateTimeslotRequest>, UpdateTimeslotValidator>();
    services.AddScoped<IValidator<RequestCustomTimeRequest>, RequestCustomTimeValidator>();
    services.AddScoped<IValidator<RespondToCustomTimeRequestRequest>, RespondToCustomTimeRequestValidator>();
    services.AddScoped<IValidator<BookTimeslotRequest>, BookTimeslotValidator>();

    // Booking validators
    services.AddScoped<IValidator<CancelBookingRequest>, CancelBookingValidator>();

    //ToDO: Remove Core Validators
    services.AddTransient<IValidator<Name>, NameValidator>();
    services.AddSingleton<IValidator<PhoneNumber>, PhoneNumberValidator>();
    services.AddSingleton<IValidator<Compensation>, CompensationValidator>();

    return services;
  }
}
