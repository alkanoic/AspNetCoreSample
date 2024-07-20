
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class ResetPasswordByEmailRequest : BaseRequest
{
    public required string UserId { get; set; }
}
