namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class FetchUserClientRolesRequest : BaseRequest
{
    public required string UserId { get; set; }
    public required string ClientUuid { get; set; }
}
