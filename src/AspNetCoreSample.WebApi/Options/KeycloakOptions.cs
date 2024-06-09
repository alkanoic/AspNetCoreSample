namespace AspNetCoreSample.WebApi.Options;

public class KeycloakOptions
{
    public required string Authority { get; set; }
    public required string Audience { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string TokenEndpoint { get; set; }
    public required string RevokeTokenEndpoint { get; set; }
    public required string AdminUserName { get; set; }
    public required string AdminPassword { get; set; }
    public required string AdminTokenEndpoint { get; set; }
    public required string AdminBaseAddress { get; set; }
    public required string TargetRealmName { get; set; }
}
