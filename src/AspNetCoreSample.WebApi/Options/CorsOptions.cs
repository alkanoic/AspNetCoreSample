namespace AspNetCoreSample.WebApi.Options;

public class CorsOptions
{
    public const string Position = "CorsOptions";

    public required string MvcUrl { get; set; }
}
