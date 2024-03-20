var builder = DistributedApplication.CreateBuilder(args);

// var apiservice = builder.AddProject<Projects.AspNetCoreSample_WebApi>("SampleWebAPI");
// builder.AddProject<Projects.AspNetCoreSample_Mvc>("SampleMvc")
//     .WithReference(apiservice);

builder.Build().Run();
