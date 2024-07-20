
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class ChangePasswordRequest : BaseRequest
{
    public required string UserId { get; set; }
    public required Credential Credential { get; set; }

}
