using Microsoft.Extensions.Logging.Console;

var builder = DistributedApplication.CreateBuilder(args);

var backend = builder.AddProject<Projects.SANSylabusApi>("backend");

var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WithReference(backend); // Zak�adaj�c, �e frontend rozmawia z backendem


builder.Build().Run();
