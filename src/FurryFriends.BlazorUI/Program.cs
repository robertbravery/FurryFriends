using FurryFriends.BlazorUI.Client.Services.Implementation;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using FurryFriends.BlazorUI.Components;
using FurryFriends.BlazorUI.Services.Implementation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Ensure Logs directory exists
var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logsDirectory))
{
  Directory.CreateDirectory(logsDirectory);
}

// Configure standard logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPetWalkerService, PetWalkerService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddSingleton<IPopupService, PopupService>();
builder.Services.AddScoped<IPictureService, PictureService>();
builder.Services.AddHttpClient<IPetWalkerService, PetWalkerService>((sp, client) =>
{
  var apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration");
  client.BaseAddress = new Uri(apiUrl);
}).AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<IClientService, ClientService>((sp, client) =>
{
  var apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration");
  client.BaseAddress = new Uri(apiUrl);
}).AddHttpMessageHandler<LoggingDelegatingHandler>();

// Register a logging delegating handler for HTTP requests
builder.Services.AddTransient<LoggingDelegatingHandler>();

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

var logger = app.Logger;
try
{
  logger.LogInformation("Starting FurryFriends BlazorUI application");
  app.Run();
}
catch (Exception ex)
{
  logger.LogCritical(ex, "FurryFriends BlazorUI application terminated unexpectedly");
}

// Partial class declaration for top-level statements
public partial class Program { }
