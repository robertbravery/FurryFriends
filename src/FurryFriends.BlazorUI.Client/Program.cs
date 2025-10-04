using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Extensions.Logging;
using FurryFriends.BlazorUI.Client.Services.Implementation;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

// Add Serilog to WebAssembly logging
builder.Logging.AddProvider(new SerilogLoggerProvider(Log.Logger));

// Add HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress) });

// Add OpenTelemetry for client-side monitoring
var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("FurryFriends.BlazorUI.Client"))
    .AddSource("FurryFriends.BlazorUI.Client")
    .Build();

// Register the client-side logging service
// This implementation only logs locally and doesn't make HTTP calls
builder.Services.AddScoped<FurryFriends.BlazorUI.Client.Services.Interfaces.IClientLoggingService, ClientLoggingServiceImpl>();

try
{
  Log.Information("Starting FurryFriends BlazorUI.Client application");
  await builder.Build().RunAsync();
}
catch (Exception ex)
{
  Log.Fatal(ex, "FurryFriends BlazorUI.Client application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}
