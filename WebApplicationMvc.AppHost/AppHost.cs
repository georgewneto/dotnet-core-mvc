var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Projeto_Web>("projeto-web");

builder.Build().Run();
