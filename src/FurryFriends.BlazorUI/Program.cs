using FurryFriends.BlazorUI.Client.Services.Implementation;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using FurryFriends.BlazorUI.Components;
using FurryFriends.BlazorUI.Services.Implementation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var host = builder.Services.AddRazorComponents()
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

app.Run();
