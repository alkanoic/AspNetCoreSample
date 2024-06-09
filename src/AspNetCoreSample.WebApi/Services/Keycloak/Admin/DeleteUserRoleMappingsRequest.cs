
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class DeleteUserRoleMappingsRequest
{
    /// <summary>
    /// ロールID
    /// </summary>
    public required string Id { get; set; }
}
