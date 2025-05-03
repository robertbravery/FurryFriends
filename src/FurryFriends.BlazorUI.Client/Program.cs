using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .CreateLogger();

// Add Serilog to WebAssembly logging
builder.Logging.AddProvider(new SerilogLoggerProvider(Log.Logger));

// Add HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress) });

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
