namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class FetchClientRolesRequest : BaseRequest
{
    public required string ClientUuid { get; set; }
}
