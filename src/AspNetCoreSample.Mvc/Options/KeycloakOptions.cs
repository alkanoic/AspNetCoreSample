namespace AspNetCoreSample.Mvc.Options;

public class KeycloakOptions
{
    public const string Position = "KeycloakOptions";

    public string Authority { get; set; } = string.Empty;
    public string MetadataAddress { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
