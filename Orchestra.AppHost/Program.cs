var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Orchestra>("orchestra");

builder.Build().Run();
