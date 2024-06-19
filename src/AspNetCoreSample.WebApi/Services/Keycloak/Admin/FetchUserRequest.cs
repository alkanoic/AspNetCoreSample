
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class FetchUserRequest : BaseRequest
{
    public required string Username { get; set; }
}
