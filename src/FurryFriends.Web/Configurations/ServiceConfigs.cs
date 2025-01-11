using FurryFriends.Core.Interfaces;
using FurryFriends.Infrastructure;
using FurryFriends.Infrastructure.Messaging;
using FurryFriends.UseCases.Configurations;
using ILogger = Serilog.ILogger;

namespace FurryFriends.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, ILogger logger, WebApplicationBuilder builder)
  {
    services.AddInfrastructureServices(builder.Configuration, logger, builder.Environment.EnvironmentName)
            .AddMediatrConfigs();
    services.AddUseCaseServices();


    if (builder.Environment.IsDevelopment())
    {
      // Use a local test email server
      // See: https://ardalis.com/configuring-a-local-test-email-server/
      services.AddScoped<IEmailSender, MimeKitEmailSender>();

      // Otherwise use this:
      //builder.Services.AddScoped<IEmailSender, FakeEmailSender>();

    }
    else
    {
      services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }

    logger.Information("{Project} services registered", "Mediatr and Email Sender");

    return services;
  }


}
