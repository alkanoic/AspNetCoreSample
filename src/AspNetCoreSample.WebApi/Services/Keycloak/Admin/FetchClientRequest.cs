namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class FetchClientRequest : BaseRequest
{
    public required string ClientId { get; set; }
}
