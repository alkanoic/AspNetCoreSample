namespace AspNetCoreSample.Mvc.Options;

public class AzureAISearchOptions
{
    public const string Position = "AzureAISearch";
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string PastQAIndexName { get; set; } = string.Empty;
    public string ManualQAIndexName { get; set; } = string.Empty;
}
