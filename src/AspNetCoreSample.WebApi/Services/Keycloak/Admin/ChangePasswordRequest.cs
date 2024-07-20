
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class ChangePasswordRequest : BaseRequest
{
    public required Credential Credential { get; set; }

}
