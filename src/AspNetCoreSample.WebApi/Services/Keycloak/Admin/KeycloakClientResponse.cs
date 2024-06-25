namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class KeycloakClientResponse
{
    public required string Id { get; set; }
    public required string ClientId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
}
