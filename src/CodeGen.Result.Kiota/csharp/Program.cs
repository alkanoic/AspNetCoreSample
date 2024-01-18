// See https://aka.ms/new-console-template for more information
using ApiSdk;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using static ApiSdk.Api.SampleTableApi.Id.IdRequestBuilder;

Console.WriteLine("Hello, World!");

// API requires no authentication, so use the anonymous
// authentication provider
var authProvider = new AnonymousAuthenticationProvider();
// Create request adapter using the HttpClient-based implementation
var adapter = new HttpClientRequestAdapter(authProvider);
// Create the API client
var client = new ApiClient(adapter);

try
{
    var sample = await client.Api.SampleTableApi.GetAsync();
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sample));

    var sampleid = await client.Api.SampleTableApi.Id.GetAsync(x =>
    {
        x.QueryParameters = new IdRequestBuilderGetQueryParameters() { Id = 1 };
    });
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sampleid));

    var multi = await client.Api.MultiTableApi.GetAsync();
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(multi));

    var multiid = await client.Api.MultiTableApi.Id.Charid.GetAsync(x =>
    {
        x.QueryParameters = new ApiSdk.Api.MultiTableApi.Id.Charid.CharidRequestBuilder.CharidRequestBuilderGetQueryParameters { Id = 1, Charid = "1" };
    });
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(multiid));
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
