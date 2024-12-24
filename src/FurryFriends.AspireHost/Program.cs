var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FurryFriends_Web>("api");

builder.Build().Run();
