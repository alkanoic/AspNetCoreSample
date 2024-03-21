namespace AspNetCoreSample.WebApi.Options;

public class CorsOptions
{
    public const string Position = "CorsOption";

    public required string MvcUrl { get; set; }
}
