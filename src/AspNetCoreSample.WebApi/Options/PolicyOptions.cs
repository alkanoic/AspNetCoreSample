namespace AspNetCoreSample.WebApi.Options;

public class PolicyOptions
{
    public TimeSpan RefreshPolicyTimeSpan { get; set; } = TimeSpan.FromSeconds(0);
    public string ClientRoleName { get; set; } = "";
}
