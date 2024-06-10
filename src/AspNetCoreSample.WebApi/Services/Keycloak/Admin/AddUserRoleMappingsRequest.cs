
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class AddUserRoleMappingsRequest
{
    /// <summary>
    /// ロールID
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// ロール名
    /// </summary>
    public required string Name { get; set; }
}
