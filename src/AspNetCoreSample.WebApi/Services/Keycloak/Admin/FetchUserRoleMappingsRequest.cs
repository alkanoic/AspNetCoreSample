
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class FetchUserRoleMappingsRequest : BaseRequest
{
    public required string UserId { get; set; }
}
