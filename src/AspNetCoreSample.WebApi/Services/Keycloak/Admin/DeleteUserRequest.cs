
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class DeleteUserRequest : BaseRequest
{
    public required string UserId { get; set; }
}
