using System.Text.Json.Serialization;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class BaseRequest
{
    [JsonIgnore]
    public string? AccessToken { get; set; }
}
