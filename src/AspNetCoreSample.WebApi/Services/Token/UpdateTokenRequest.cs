namespace AspNetCoreSample.WebApi.Services.Token;

public class UpdateTokenRequest
{
    public required string RefreshToken { get; set; }
}
