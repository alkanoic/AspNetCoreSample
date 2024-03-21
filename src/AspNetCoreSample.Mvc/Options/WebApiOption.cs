namespace AspNetCoreSample.Mvc.Options;

public class WebApiOption
{
    public const string Position = "WebApiOption";

    public required string WebApiBaseUrl { get; set; }
}
