using FurryFriends.UseCases.Configurations;
using FurryFriends.Web.Configurations;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddLogging(logging =>
{
  logging.AddConsole();
  logging.AddOpenTelemetry();
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());


var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

//builder.Services.AddHttpContextAccessor();

// Register services
builder.Services
    .AddOptionConfigs(builder.Configuration, logger, builder)
    .AddValidatorConfigs()
    .AddUseCaseValidators()
    .AddMediatrConfigs();

builder.Services.AddServiceConfigs(logger, builder);

// Add FastEndpoints
builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });

var app = builder.Build();

await app.UseAppMiddlewareAndSeedDatabase();

app.UseFastEndpoints(c =>
{
  c.Endpoints.RoutePrefix = "api";
  c.Errors.UseProblemDetails(x =>
  {
    x.AllowDuplicateErrors = true;
    x.IndicateErrorCode = true;
    x.IndicateErrorSeverity = true;
    x.TypeValue = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1";
    x.TitleValue = "One or more validation errors occurred.";
    x.TitleTransformer = pd => pd.Status switch
    {
      400 => "Validation Error",
      404 => "Not Found",
      _ => "One or more errors occurred!"
    };
  });
});

app.Run();

public partial class Program { }
