
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class FetchRoleResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
