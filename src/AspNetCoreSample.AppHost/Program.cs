var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin(x => x.WithHostPort(8090));//.WithPgWeb(x => x.WithHostPort(8091));

var postgresdb = postgres.AddDatabase("postgresdb");

var apiservice = builder.AddProject<Projects.AspNetCoreSample_WebApi>("SampleWebAPI")
                        .WithReference(postgresdb);
builder.AddProject<Projects.AspNetCoreSample_Mvc>("SampleMvc")
    .WithReference(apiservice)
    .WithReference(postgresdb);

builder.Build().Run();
