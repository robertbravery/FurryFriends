using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//var apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });

await builder.Build().RunAsync();
