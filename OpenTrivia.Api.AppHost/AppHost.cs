var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OpenTrivia_Api>("opentrivia-api");

builder.Build().Run();
