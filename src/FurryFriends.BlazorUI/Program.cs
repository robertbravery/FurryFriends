using FurryFriends.BlazorUI.Client.Services.Implementation;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using FurryFriends.BlazorUI.Components;
using FurryFriends.BlazorUI.Services.Implementation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Ensure Logs directory exists
var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logsDirectory))
{
  Directory.CreateDirectory(logsDirectory);
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logsDirectory, "blazorui-log-.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        fileSizeLimitBytes: 10 * 1024 * 1024,
        retainedFileCountLimit: 31,
        rollOnFileSizeLimit: true)
    .CreateLogger();

// Use Serilog for logging
builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPetWalkerService, PetWalkerService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddSingleton<IPopupService, PopupService>();
builder.Services.AddScoped<IPictureService, PictureService>();
builder.Services.AddScoped<FurryFriends.BlazorUI.Client.Services.Interfaces.IClientLoggingService, ServerClientLoggingService>();

// Register the LoggingDelegatingHandler for HTTP request/response logging
builder.Services.AddTransient<LoggingDelegatingHandler>();

// Configure service discovery and resilience manually instead of using AddServiceDefaults
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(http =>
{
    // Turn on resilience by default
    http.AddStandardResilienceHandler();

    // Turn on service discovery by default
    http.AddServiceDiscovery();
});

// Configure HttpClient with service discovery
builder.Services.AddHttpClient<IPetWalkerService, PetWalkerService>((sp, client) =>
{
    // Use service discovery to find the API
    var config = sp.GetService<IConfiguration>();
    var apiBaseUrl = config?["ApiBaseUrl"];
    client.BaseAddress = !string.IsNullOrEmpty(apiBaseUrl)
        ? new Uri(apiBaseUrl)
        : new Uri("http://api");
}).AddHttpMessageHandler<LoggingDelegatingHandler>();

// Configure other HttpClients similarly
builder.Services.AddHttpClient<IClientService, ClientService>((sp, client) =>
{
    var config = sp.GetService<IConfiguration>();
    var apiBaseUrl = config?["ApiBaseUrl"];
    client.BaseAddress = !string.IsNullOrEmpty(apiBaseUrl)
        ? new Uri(apiBaseUrl)
        : new Uri("http://api");
}).AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<ILocationService, LocationService>((sp, client) =>
{
  var apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration");
  client.BaseAddress = new Uri(apiUrl);
}).AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<IPictureService, PictureService>((sp, client) =>
{
  var apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration");
  client.BaseAddress = new Uri(apiUrl);
}).AddHttpMessageHandler<LoggingDelegatingHandler>();

// Configure HttpClient for the server-side logging service
builder.Services.AddHttpClient<ServerClientLoggingService>((sp, client) =>
{
  var apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration");
  client.BaseAddress = new Uri(apiUrl);
}).AddHttpMessageHandler<LoggingDelegatingHandler>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(FurryFriends.BlazorUI.Client._Imports).Assembly);

try
{
  Log.Information("Starting FurryFriends BlazorUI application");
  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "FurryFriends BlazorUI application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}

// Partial class declaration for top-level statements
public partial class Program { }
