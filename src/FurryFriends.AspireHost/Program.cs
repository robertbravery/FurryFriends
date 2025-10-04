var builder = DistributedApplication.CreateBuilder(args);

// Add existing Web API project
//var api = builder.AddProject<Projects.FurryFriends_Web>("api");

// Add Blazor UI project (server-side hosting with WASM)
//builder.AddProject<Projects.FurryFriends_BlazorUI>("blazorui")
//.WithReference(api);

builder.Build().Run();
