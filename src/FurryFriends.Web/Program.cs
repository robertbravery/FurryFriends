﻿using Ardalis.ListStartupServices;
using FurryFriends.ServiceDefaults;
using FurryFriends.UseCases.Configurations;
using FurryFriends.Web.Configurations;
using FurryFriends.Web.Middleware;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add CORS configuration
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowBlazorClient",
      builder => builder
          .WithOrigins(
              "https://localhost:7214",  // Blazor WASM client URL
              "http://localhost:5185"    // HTTP version
          )
          .AllowAnyMethod()
          .AllowAnyHeader()
          .WithExposedHeaders("*"));
});

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

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
}
else
{
  app.UseDefaultExceptionHandler();
  app.UseHsts();
}

// Add CORS middleware - must be before routing and endpoints
app.UseCors("AllowBlazorClient");

app.UseHttpsRedirection();

await app.UseAppMiddlewareAndSeedDatabase();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseFastEndpoints(
  c =>
{
  c.Endpoints.RoutePrefix = "api";
  c.Errors.UseProblemDetails(x =>
  {
    x.AllowDuplicateErrors = false;
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
}
).UseSwaggerGen();


app.Run();

public partial class Program { }
